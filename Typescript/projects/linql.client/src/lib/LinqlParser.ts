import { LinqlConstant, LinqlExpression, LinqlLambda, LinqlObject, LinqlParameter, LinqlProperty, LinqlType, LinqlUnary } from "linql.core";
import { AnyExpression } from "./Types";
import * as Acorn from 'acorn';
import * as AcornWalk from 'acorn-walk';
import * as ESTree from 'estree';



export class LinqlParser 
{
    protected LinqlStack: Array<LinqlExpression> = new Array<LinqlExpression>();

    protected ExpressionStack: Array<Acorn.Node> = new Array<Acorn.Node>();

    Root: LinqlExpression | undefined;

    constructor(protected RootExpression: AnyExpression<any> | string | undefined | Acorn.Node)
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
                debugger;
            },
            MemberExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                State.VisitMember(Node, Callback);
            },
            LogicalExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                debugger;
            },
            BinaryExpression(Node: Acorn.Node, State: LinqlParser, Callback: AcornWalk.WalkerCallback<LinqlParser>)
            {
                debugger;
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

        const bodyParser = new LinqlParser(lambda.body as Acorn.Node);
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
        const param = new LinqlParameter(node.name);
        this.AttachToExpression(param);
        this.PushToStack(param, Node);

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

            if (previous instanceof LinqlConstant && previous.Value && !previous.ConstantType?.IsList())
            {

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

}
