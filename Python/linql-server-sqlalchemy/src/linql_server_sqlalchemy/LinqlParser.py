from types import CodeType
from linql_core.LinqlExpression import LinqlExpression
from linql_core.LinqlLambda import LinqlLambda
from linql_core.LinqlConstant import LinqlConstant
from linql_core.LinqlType import LinqlType
from linql_core.LinqlObject import LinqlObject
from linql_core.LinqlUnary import LinqlUnary
from linql_core.LinqlFunction import LinqlFunction
from linql_core.LinqlParameter import LinqlParameter
from linql_core.LinqlBinary import LinqlBinary
from linql_core.LinqlProperty import LinqlProperty
from linql_core.ITypeNameProvider import ITypeNameProvider
from typing import Any
from collections import namedtuple
import inspect
import dis

Lambda = namedtuple('Lambda', ['args', 'expr'])
Val = namedtuple('Val', ['v'])
Arg = namedtuple('Arg', ['n'])
Global = namedtuple('Global', ['n'])
BinOp = namedtuple('BinOp', ['op', 'x', 'y'])
UnOp = namedtuple('UnOp', ['op', 'x'])
IfElse = namedtuple('IfElse', ['c', 't', 'f'])
# TODO maybe Subscr
Attr = namedtuple('Attr', ['n', 'x'])
List = namedtuple('List', ['vs'])
Tuple = namedtuple('Tuple', ['vs'])
Map = namedtuple('Map', ['vs'])
Set = namedtuple('Set', ['vs'])
Call = namedtuple('Call', ['f', 'args'])
#CallKw = namedtuple('Call', ['f', 'args', 'kw'])

class LinqlParser:

    LinqlStack: list[LinqlExpression] = []
    ExpressionStack: list[Any]
    Root: LinqlExpression | None
    RootExpression: Any | None
    TypeNameProvider: ITypeNameProvider

    def __init__(self, Expression: Any, TypeNameProvider: ITypeNameProvider) -> None:
        self.RootExpression = Expression
        self.TypeNameProvider = TypeNameProvider
        self.Root = self.Visit(self.RootExpression)
            

    def _to_str(self, ex):
        if type(ex) is Lambda:
            if ex.args:
                return 'lambda {}: {}'.format(', '.join(ex.args), self._to_str(ex.expr))
            return 'lambda: {}'.format(self._to_str(ex.expr))
        if type(ex) is Val:
            return str(ex.v)
        if type(ex) in (Arg, Global):
            return str(ex.n)
        if type(ex) is Attr:
            p = self._get_prec(ex)
            if p > self._get_prec(ex.x):
                return '({}).{}'.format(self._to_str(ex.x), ex.n)
            return '{}.{}'.format(self._to_str(ex.x), ex.n)
        if type(ex) is BinOp:
            p = self._get_prec(ex)
            if ex.op == '[]':
                if p > self._get_prec(ex.x):
                    return '({})[{}]'.format(self._to_str(ex.x), self._to_str(ex.y))
                return '{}[{}]'.format(self._to_str(ex.x), self._to_str(ex.y))
            fx = '{}' if p <= self._get_prec(ex.x)&62 else '({})'
            fy = '{}' if p|1 <= self._get_prec(ex.y) else '({})'
            f = fx + ' {} ' + fy
            return f.format(self._to_str(ex.x), ex.op, self._to_str(ex.y))
        if type(ex) is UnOp:
            t = ex.op
            if t[-1].isalpha():
                t = t + ' '
            if self._get_prec(ex) > self._get_prec(ex.x):
                return '{}({})'.format(t, self._to_str(ex.x))
            return '{}{}'.format(t, self._to_str(ex.x))
        if type(ex) is Call:
            if self._get_prec(ex) > self._get_prec(ex.f):
                return '({})({})'.format(self._to_str(ex.f), ', '.join(map(self._to_str, ex.args)))
            return '{}({})'.format(self._to_str(ex.f), ', '.join(map(self._to_str, ex.args)))
        if type(ex) is IfElse:
            p = self._get_prec(ex)
            fc = '{}' if p < self._get_prec(ex.c) else '({})'
            ft = '{}' if p < self._get_prec(ex.t) else '({})'
            ff = '{}' if p <= self._get_prec(ex.f) else '({})'
            f = '{} if {} else {}'.format(ft, fc, ff)
            return f.format(self._to_str(ex.t), self._to_str(ex.c), self._to_str(ex.f))
        if type(ex) is List:
            return '[{}]'.format(', '.join(map(self._to_str, ex.vs)))
        if type(ex) is Tuple:
            if len(ex.vs) == 1:
                return '({},)'.format(self._to_str(ex.vs[0]))
            return '({})'.format(', '.join(map(self._to_str, ex.vs)))
        if type(ex) is Set:
            if len(ex.vs) == 0:
                return 'set()'
            return '{{{}}}'.format(', '.join(map(self._to_str, ex.vs)))
        if type(ex) is Map:
            return '{{{}}}'.format(', '.join(map(lambda p: '{}: {}'.format(self._to_str(p[0]), self._to_str(p[1])), ex.vs)))
        raise TypeError(type(ex))

    __bin_prec = {
        'or': 5,
        'and': 7,
        'in': 10, 
        'is': 10, 
        'not in': 10, 
        'is not': 10,
        '<': 10, 
        '<=': 10, 
        '>': 10, 
        '>=': 10, 
        '==': 10, 
        '!=': 10,
        '|': 12,
        '^': 14,
        '&': 16,
        '<<': 18, 
        '>>': 18,
        '+': 20, 
        '-': 20,
        '*': 22, 
        '@': 22, 
        '/': 22, 
        '//': 22, 
        '%': 22,
        '**': 27,
        '[]': 30,
    }

    _un_prec = {
        'not': 11,
        '+': 25, 
        '-': 25, 
        '~': 25,
    }

    def _get_prec(self, ex):
        if type(ex) is BinOp:
            return self.__bin_prec[ex.op]
        if type(ex) is UnOp:
            return self._un_prec[ex.op]
        if type(ex) is Lambda:
            return 0
        if type(ex) is IfElse:
            return 2
        if type(ex) in (Attr, Call):
            return 30
        if type(ex) in (List, Tuple, Map, Set):
            return 32
        return 63

    __unary_lookup = {
        'UNARY_POSITIVE': '+',
        'UNARY_NEGATIVE': '-',
        'UNARY_NOT': 'Not',
        'UNARY_INVERT': '~',
        #'GET_ITER': 'GET_ITER',
        #'GET_YIELD_FROM_ITER': 'GET_YIELD_FROM_ITER',
    }

    _binary_lookup = {
        'BINARY_POWER': '**',
        'BINARY_MULTIPLY': '*',
        'BINARY_MATRIX_MULTIPLY': '@',
        'BINARY_FLOOR_DIVIDE': '//',
        'BINARY_TRUE_DIVIDE': '/',
        'BINARY_MODULO': '%',
        'BINARY_ADD': '+',
        'BINARY_SUBTRACT': '-',
        'BINARY_SUBSCR': '[]',
        'BINARY_LSHIFT': '<<',
        'BINARY_RSHIFT': '>>',
        'BINARY_AND': '&',
        'BINARY_XOR': '^',
        'BINARY_OR': '|',
    }

    _builtInLookup = {
        "any": "Any",
        "filter": "Where",
        "map": "Select",
        "selectMany": "SelectMany",
        "lower": "ToLower"
    }

    def _find_offset(self, ops, offset):
        i, k = 0, len(ops)
        while i < k:
            j = (i + k) // 2
            o = ops[j].offset
            if o == offset:
                return j
            if o > offset:
                k = j
            else:
                i = j + 1
        if k == len(ops):
            return k
        raise KeyError

    def _normalize(self, x):
        if type(x) is IfElse:
            if type(x.t) is BinOp and x.t.op == 'or' and x.t.y == x.f:
                # c or d if b else d --> c and b or d
                return BinOp('or', BinOp('and', x.c, x.t.x), x.f)
            if type(x.t) is BinOp and x.t.op == 'and' and x.t.y == x.f and type(x.c) is UnOp and x.c.op == 'not':
                # b and c if not a else c --> (a or b) and c
                return BinOp('and', BinOp('or', x.c.x, x.t.x), x.f)
        return x

    def VisitLambda(self, f) -> LinqlLambda:
        assert inspect.isfunction(f)
        args = list(map(lambda x: LinqlParameter(x), inspect.signature(f).parameters.keys()))
        # TODO assert no *args, **kwargs
        expr = self._parse_expr(list(dis.get_instructions(f)), 0, [])
        linqlLambda = LinqlLambda()
        linqlLambda.Body = expr
        linqlLambda.Parameters = args
        return linqlLambda

    def VisitConstant(self, op: Any) -> LinqlConstant:
        pass


    opTable = []

    def NullableCheck(self, left: LinqlExpression, right: LinqlExpression):
        
        leftConstant = isinstance(left, LinqlConstant)
        rightConstant = isinstance(right, LinqlConstant) 
        nullableCheck: LinqlExpression | None = None
        if leftConstant and left.Value == None and rightConstant == False:
            nullableCheck = right
        elif rightConstant and right.Value == None and leftConstant == False:
            nullableCheck = left

        if nullableCheck != None:
            lastExp = nullableCheck.GetLastExpressionInNextChain()
            prop = LinqlProperty("HasValue")
            lastExp.Next = prop

        return nullableCheck

    def _parse_expr(self, ops, i, stack, stackModifier: int = 0):
        for j in range(i, len(ops)):
            op = ops[j]
            opname = op.opname
            if opname == 'RETURN_VALUE':
                return stack[-1]
            if opname == 'LOAD_CONST':
                value = op.argval
                
                if isinstance(value, CodeType):
                    stack.append(value)
                else:
                    linqlType = LinqlType.GetLinqlType(value, self.TypeNameProvider)
                    linqlConstant = LinqlConstant(linqlType, value)
                    stack.append(linqlConstant)
                continue
            if opname == 'LOAD_FAST':
                parameter = LinqlParameter(op.argval)
                stack.append(parameter)
                continue
            #if opname == 'LOAD_GLOBAL':
            if opname in ('LOAD_DEREF'):
                value = inspect.stack()[7][0].f_locals[op.argval]
                
                if isinstance(value, LinqlObject):
                    linqlExpression = value
                    pass
                else:
                    linqlType = LinqlType.GetLinqlType(value, self.TypeNameProvider)
                    linqlExpression = LinqlConstant(linqlType, value)
                stack.append(linqlExpression)
                continue
            if opname in ('LOAD_GLOBAL', 'LOAD_CLOSURE', "LOAD_METHOD"):
                globalVars = inspect.stack()[7 + stackModifier][0].f_globals
                builtIns = __builtins__
                instanceFunction = False
                if op.argval in globalVars:
                    value = globalVars[op.argval]
                  
                elif op.argval in builtIns:
                    value = builtIns[op.argval]
                elif op.argval in self._builtInLookup:
                    instanceFunction = True
                    value = op.argval

                if callable(value) or instanceFunction == True:
                    if callable(value):
                        funName = value.__name__
                    else: 
                        funName = op.argval
                    if funName in self._builtInLookup:
                        funName = self._builtInLookup[funName]
                    fun = LinqlFunction(funName, [])
                    stack.append(fun)
                else:
                    linqlType = LinqlType.GetLinqlType(value, self.TypeNameProvider)
                    linqlConstant = LinqlConstant(linqlType, value)
                    stack.append(linqlConstant)
                continue
            if opname == 'LOAD_ATTR':
                x: LinqlExpression = stack.pop()
                last = x.GetLastExpressionInNextChain()

                if isinstance(last, LinqlConstant):
                    value = getattr(last.Value, op.argval)
                    linqlType = LinqlType.GetLinqlType(value, self.TypeNameProvider)
                    x = LinqlConstant(linqlType, value)
                elif isinstance(last, LinqlObject) and op.argval == "Value":
                    pass
                else:
                    property = LinqlProperty(op.argval)
                    last.Next = property
                stack.append(x)
                continue
            tag = self.__unary_lookup.get(opname, None)
            if tag:
                x: LinqlExpression = stack.pop()
                unary = LinqlUnary(tag, None)
                unary.Next = x     
                stack.append(unary)
                continue
            tag = self._binary_lookup.get(opname, None)
            if tag:
                y = stack.pop()
                x = stack.pop()
                bin = LinqlBinary(tag, x, y)
                stack.append(bin)
                continue
            if opname == 'COMPARE_OP':
                y = stack.pop()
                x = stack.pop()

                if op.argval == "!=":
                   bin = LinqlBinary("NotEqual", x, y)
                else:
                    bin = LinqlBinary("Equal", x, y)

                nullCheck = self.NullableCheck(bin.Left, bin.Right)

                if nullCheck != None:
                    bin = nullCheck

                stack.append(bin)
                continue
            if opname == 'CONTAINS_OP':
                x = stack.pop()
                y = stack.pop()
                last = x.GetLastExpressionInNextChain()
                fun = LinqlFunction("Contains", [y])
                last.Next = fun

                if op.arg == 1:
                    notUnary = LinqlUnary("Not", [x])
                    x = notUnary

                stack.append(x)
                continue
            if opname == 'JUMP_IF_FALSE_OR_POP':
                jj = self._find_offset(ops, op.argval)
                a: LinqlExpression = stack.pop()
                b = self._parse_expr(ops[:jj], j + 1, stack[:], stackModifier + 1)

                lastA = a.GetLastExpressionInNextChain()

                if isinstance(lastA, LinqlProperty) and lastA.PropertyName == "HasValue":
                    bin = LinqlBinary("AndAlso", a, b)
                else:
                    bin = LinqlBinary("AndAlso", b, a)

                stack.append(bin)
                return self._parse_expr(ops, jj, stack, stackModifier + 1)
            if opname == 'JUMP_IF_TRUE_OR_POP':
                jj = self._find_offset(ops, op.argval)
                a = stack.pop()
                b = self._parse_expr(ops[:jj], j + 1, stack[:], stackModifier + 1)
                bin = LinqlBinary("Or", a, b)
                stack.append(bin)
                return self._parse_expr(ops, jj, stack, stackModifier + 1)
            if opname == 'POP_JUMP_IF_FALSE':
                jj = self._find_offset(ops, op.argval)
                k = None
                if ops[jj - 1].opname == 'JUMP_FORWARD':
                    k = self._find_offset(ops, ops[jj - 1].argval)
                c = stack.pop()
                if k is None:
                    ##t = self._parse_expr(ops[:jj], j + 1, stack[:])
                    t = self._parse_expr(ops, j + 1, stack[:], stackModifier + 1)
                    f = self._parse_expr(ops, jj, stack, stackModifier + 1)
                    return self._normalize(IfElse(c, t, f))
                else:
                    t = self._parse_expr(ops[:jj - 1], j + 1, stack[:], stackModifier + 1)
                    f = self._parse_expr(ops[:k], jj, stack[:], stackModifier + 1)
                    stack.append(self._normalize(IfElse(c, t, f)))
                    return self._parse_expr(ops[k:], 0, stack)
            if opname == 'POP_JUMP_IF_TRUE':
                jj = self._find_offset(ops, op.argval)
                k = None
                if ops[jj - 1].opname == 'JUMP_FORWARD':
                    k = self._find_offset(ops, ops[jj - 1].argval)
                c = stack.pop()
                if k is None:
                    ##t = self._parse_expr(ops[:jj], j + 1, stack[:])
                    t = self._parse_expr(ops, j + 1, stack[:], stackModifier + 1)
                    f = self._parse_expr(ops, jj, stack, stackModifier + 1)
                    return self._normalize(IfElse(UnOp('not', c), t, f))
                else:
                    t = self._parse_expr(ops[:jj - 1], j + 1, stack[:], stackModifier + 1)
                    f = self._parse_expr(ops[:k], jj, stack[:], stackModifier + 1)
                    stack.append(self._normalize(IfElse(UnOp('not', c), t, f)))
                    return self._parse_expr(ops[k:], 0, stack, stackModifier + 1)
            if opname == 'JUMP_FORWARD':
                jj = self._find_offset(ops, op.argval)
                return self._parse_expr(ops, jj, stack, stackModifier + 1)
            if opname == 'BUILD_LIST':
                vs = tuple(self._popn(stack, op.argval))
                stack.append(List(vs))
                continue
            if opname == 'BUILD_TUPLE':
                vs = tuple(self._popn(stack, op.argval))
                stack.append(Tuple(vs))
                continue
            if opname == 'BUILD_SET':
                vs = tuple(self._popn(stack, op.argval))
                stack.append(Set(vs))
                continue
            if opname == 'BUILD_MAP':
                vs = self._popn(stack, 2*op.argval)
                vs = tuple(zip(vs[0::2], vs[1::2]))
                stack.append(Map(vs))
                continue
            if opname == 'CALL_FUNCTION':
                args = tuple(self._popn(stack, op.argval))
                f = stack.pop()
                stack.append(Call(f, args))
                continue
            #if opname == 'CALL_FUNCTION_KW':
            #    kw = stack.pop()
            #    args = tuple(self._popn(stack, op.argval))
            #    f = stack.pop()
            #    assert type(kw) is Val
            #    stack.append(CallKw(f, args, kw.v))
            #    continue
            # TODO CALL_FUNCTION_EX, BUILD_TUPLE_UNPACK_WITH_CALL
            if opname == 'MAKE_FUNCTION':
                assert op.argval in (0, 8)
                co = stack.pop()
                if op.argval & 8:
                    stack.pop()  # discard closed over variables
                args = co.co_varnames
                expr = self._parse_expr(list(dis.get_instructions(co)), 0, [], stackModifier + 1)
                lambdaFun = LinqlLambda()
                lambdaFun.Body = expr
                lambdaFun.Parameters = list(map(lambda param: LinqlParameter(param), args))
                stack.append(lambdaFun)
                continue
            if opname == "CALL":
                argCount = op.arg
                argList = []
                while argCount > 0:
                    argList.append(stack.pop())
                    argCount -= 1
                
                fun: LinqlFunction = stack.pop()

                if op.arg > 0:
                    funArg = argList.pop()
                    iterator: LinqlExpression = argList.pop()
                    fun.Arguments = [funArg]
                else:
                    iterator: LinqlExpression = stack.pop()
                    fun.Arguments = []
                iterator.GetLastExpressionInNextChain().Next = fun
                stack.append(iterator)
                continue
            # TODO maybe do comprehension
            if opname in ['RESUME', 'NOP', "COPY_FREE_VARS", "PRECALL"]:
                continue
            if opname == 'POP_TOP':
                stack.pop()
                continue
            if opname == 'ROT_TWO':
                t = stack[-2]
                stack[-2] = stack[-1]
                stack[-1] = t
                continue
            if opname == 'ROT_THREE':
                t = stack[-1]
                stack[-1] = stack[-2]
                stack[-2] = stack[-3]
                stack[-3] = t
                continue
            if opname == 'DUP_TOP':
                stack.append(stack[-1])
                continue
            if opname == 'DUP_TOP_TWO':
                stack.append(stack[-2])
                stack.append(stack[-2])
                continue
            raise ValueError(op.opname)
        return stack[-1]

    def _popn(self, l, n):
        if not n:
            return []
        r = l[-n:]
        del l[-n:]
        return r        
    
    def Visit(self, exp: Any):
        if exp == None:
            return
        else:
           return self.VisitLambda(exp)
