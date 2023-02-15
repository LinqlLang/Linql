import { LinqlBinary, LinqlConstant, LinqlExpression, LinqlFunction, LinqlLambda, LinqlObject, LinqlParameter, LinqlProperty, LinqlType, LinqlUnary } from "linql.core";
import { AnyExpression } from "./Types";
import * as Acorn from 'acorn';
import * as AcornWalk from 'acorn-walk';
import * as ESTree from 'estree';



export class LinqlParser 
{
    protected LinqlStack: Array<LinqlExpression> = new Array<LinqlExpression>();

    protected ExpressionStack: Array<Acorn.Node> = new Array<Acorn.Node>();

    Root: LinqlExpression | undefined;

    constructor(protected RootExpression: AnyExpression<any> | string | undefined | Acorn.Node, protected ArgumentContext: {})
    {
        this.Visit();
    }

    protected PushToStack(Expression: LinqlExpression, Node: Acorn.Node)
    {
        this.LinqlStack.push(Expression);
        this.ExpressionStack.push(Node);
    }

    protected PopStack()
    {
        this.LinqlStack.pop();
        this.ExpressionStack.pop();
    }

    AttachToExpression(ExpressionToAttach: LinqlExpression)
    {
        const previousExpression = this.LinqlStack.at(-1);
        if (previousExpression)
        {
            previousExpression.Next = ExpressionToAttach;
        }
        else
        {
            this.Root = ExpressionToAttach;
        }
    }

    RemoveFromPrevious(Expression: LinqlExpression | undefined)
    {
        if (Expression)
        {
            this.PopStack();
        }
    }


    protected Visit()
    {
        if (!this.RootExpression)
        {
            return;
        }
        let expression: Acorn.Node | undefined;

        if (this.RootExpression instanceof Acorn.Node)
        {
            expression = this.RootExpression;
        }
        else
        {
            let rootString: string | Acorn.Node;

            if (typeof this.RootExpression === "string")
            {
                rootString = this.RootExpression;
            }
            else
            {
                rootString = this.RootExpression.toLocaleString();
            }
            expression = Acorn.parse(rootString, { ecmaVersion: 'latest' });

        }

        AcornWalk.recursive(expression, this, {
            ArrowFunctionExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitLambda(Node, Callback);
            },
            CallExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitMethodCall(Node, Callback);
            },
            MemberExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitMember(Node, Callback);
            },
            LogicalExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitBinary(Node, Callback);
            },
            BinaryExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitBinary(Node, Callback);
            },
            UnaryExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitUnary(Node, Callback);
            },
            Identifier(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitParameter(Node, Callback);
            },
            Literal(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitConstant(Node, Callback);
            }
        });
    }

    VisitLambda(Node: Acorn.Node, Callback: AcornWalk.WalkerCallback<LinqlParser>)
    {
        const linqlLambda = new LinqlLambda();
        const lambda = Node as any as ESTree.ArrowFunctionExpression;

        this.AttachToExpression(linqlLambda);
        this.PushToStack(linqlLambda, Node);


        lambda.params.forEach(r =>
        {
            const cast = r as ESTree.Identifier;
            const param = new LinqlParameter(cast.name);

            if (!linqlLambda.Parameters)
            {
                linqlLambda.Parameters = new Array<LinqlExpression>();
            }
            linqlLambda.Parameters.push(param);
        });

        const bodyParser = new LinqlParser(lambda.body as Acorn.Node, this.ArgumentContext);
        linqlLambda.Body = bodyParser.Root;
    }

    VisitConstant(Node: Acorn.Node, Callback: AcornWalk.WalkerCallback<LinqlParser>)
    {
        const cast = Node as any as ESTree.Literal;
        const value = cast.value;
        const type = LinqlType.GetLinqlType(value);
        const constant = new LinqlConstant(type, value);
        this.AttachToExpression(constant);
        this.PushToStack(constant, Node);
    }

    VisitParameter(Node: Acorn.Node, Callback: AcornWalk.WalkerCallback<LinqlParser>)
    {
        const node = Node as any as ESTree.Identifier;
        const anyArg = this.ArgumentContext as any;
        let expression: LinqlExpression;

        if (node.name.startsWith("this") || node.name.startsWith("_this"))
        {
            let value = anyArg[node.name];

            if (!value)
            {
                value = anyArg["this"];
            }

            if (!value)
            {
                throw `Unable to extract argument ${ Node }`;
            }
            else
            {
                expression = new LinqlConstant(LinqlType.GetLinqlType(value), value);
            }
        }
        else
        {
            expression = new LinqlParameter(node.name);
        }
        this.AttachToExpression(expression);
        this.PushToStack(expression, Node);


    }

    VisitMember(Node: Acorn.Node, Callback: AcornWalk.WalkerCallback<LinqlParser>)
    {
        const node = Node as any as ESTree.MemberExpression;

        let memberName: string | undefined;

        switch (node.property.type)
        {
            case "Identifier":
            default:
                memberName = (node.property as ESTree.Identifier).name;
                break;
        }

        Callback(node.object as Acorn.Node, this);
        const previous = this.LinqlStack.at(-1);

        if (memberName)
        {
            const property = new LinqlProperty(memberName);
            let expression: LinqlExpression | undefined;
            if (previous instanceof LinqlConstant && previous.Value && !previous.ConstantType?.IsList())
            {
                let value = previous.Value;

                if (value != null)
                {
                    value = value[memberName];
                }

                if (value != null)
                {
                    const type = LinqlType.GetLinqlType(value);

                    if (value instanceof LinqlObject)
                    {

                    }
                    else
                    {
                        expression = new LinqlConstant(type, value);
                    }
                }
                else
                {
                    expression = new LinqlConstant(LinqlType.GetLinqlType(null), null);
                }

                if (expression)
                {
                    this.RemoveFromPrevious(previous);
                    this.AttachToExpression(expression);
                    this.PushToStack(expression, Node);
                }
                else
                {
                    throw `Unalbe to parse member from constant ${ previous } ${ node }`;
                }
            }
            else if (previous instanceof LinqlObject)
            {

            }
            else
            {
                this.AttachToExpression(property);
                this.PushToStack(property, Node);
            }

        }
        else
        {
            throw `Was unable to find Member for ${ node }`;
        }


    }

    VisitUnary(Node: Acorn.Node, Callback: AcornWalk.WalkerCallback<LinqlParser>)
    {
        const node = Node as any as ESTree.UnaryExpression;
        let unary: LinqlUnary | undefined;
        switch (node.operator)
        {
            case "!":
                unary = new LinqlUnary("Not");
                break;
            default:
                break;
        }

        if (unary)
        {
            this.AttachToExpression(unary);
            this.PushToStack(unary, Node);
            Callback(node.argument as Acorn.Node, this);
        }
        else
        {
            throw `Unable interpret unary operator ${ Node }`;
        }
    }

    VisitBinary(Node: Acorn.Node, Callback: AcornWalk.WalkerCallback<LinqlParser>)
    {
        const node = Node as any as ESTree.BinaryExpression;
        let binary: LinqlBinary | undefined;
        const operator: ESTree.BinaryOperator | "&&" = node.operator as ESTree.BinaryOperator | "&&";

        switch (operator)
        {
            case "==":
            case "===":
                binary = new LinqlBinary("Equal");
                break;
            case "&":
            case "&&":
                binary = new LinqlBinary("AndAlso");
                break;
            default:
                break;

        }

        if (binary)
        {
            this.AttachToExpression(binary);
            this.PushToStack(binary, Node);

            const left = node.left;
            const right = node.right;

            const leftParser = new LinqlParser(left as Acorn.Node, this.ArgumentContext);
            const rightParser = new LinqlParser(right as Acorn.Node, this.ArgumentContext);

            binary.Left = leftParser.Root;
            binary.Right = rightParser.Root;
        }
        else
        {
            throw `Unable to find binary expression ${ node.operator }`;
        }
    }

    VisitMethodCall(Node: Acorn.Node, Callback: AcornWalk.WalkerCallback<LinqlParser>)
    {
        const node = Node as any as ESTree.CallExpression;
        const callee = node.callee as ESTree.MemberExpression;
        const functionName = callee.property as ESTree.Identifier;
        const linqlFunction = new LinqlFunction(functionName.name);
        let functionCallee: LinqlExpression | undefined;

        linqlFunction.Arguments = node.arguments.map(s =>
        {
            const parser = new LinqlParser(s as Acorn.Node, this.ArgumentContext);
            return parser.Root;
        }) as Array<LinqlExpression>;

        if (callee.object)
        {
            const parser = new LinqlParser(callee.object as Acorn.Node, this.ArgumentContext);
            functionCallee = parser.Root;
        }

        if (functionCallee)
        {
            const attachTo = functionCallee.GetLastExpressionInNextChain();
            attachTo.Next = linqlFunction;
            this.Root = functionCallee;
        }
    }


}
