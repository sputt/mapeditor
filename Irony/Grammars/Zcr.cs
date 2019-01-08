using System;
using System.Collections.Generic;
using System.Linq;
using Irony.Parsing;

namespace Demo
{
    // MyC language, simplified c-like language originally used as an example in VS SDK
    // The following grammar is based on Grammar class by Ben Morrison
    // from his article and sample project "Writing Your First Visual Studio Language Service"
    //    http://www.codeproject.com/Articles/33250/Writing-Your-First-Visual-Studio-Language-Service
    // Slightly edited - added a hint and fixed one expression

    [Language("Zelda", "0.3", "Zelda Scripting Language")]
    public class ZcrGrammar : Irony.Parsing.Grammar
    {
        public ZcrGrammar()
        {
            #region Declare Terminals Here
            CommentTerminal lineComment = new CommentTerminal("line-comment", ";;",
                "\r", "\n", "\u2085", "\u2028", "\u2029");
            NonGrammarTerminals.Add(lineComment);

            NumberLiteral number = new NumberLiteral("number");
            IdentifierTerminal identifier = new IdentifierTerminal("identifier");
            #endregion

            #region Declare NonTerminals Here
            
            var script = new NonTerminal("script");
            var statements = new NonTerminal("statements");
            var statement = new NonTerminal("statement");
            var label = new NonTerminal("label");
            var labelRef = new NonTerminal("label-ref");
            var variableRef = new NonTerminal("variable-ref");

            var assignmentType = new NonTerminal("assignment-type");
            var assignment = new NonTerminal("assignment");

            var arrayAccess = new NonTerminal("array-access");
            var objectType = new NonTerminal("object-type");

            var structAccess = new NonTerminal("struct-access");
            var lvalue = new NonTerminal("lvalue");

            var expression = new NonTerminal("expression");
            var simpleExpression = new NonTerminal("simple-expression");
            var mathExpression = new NonTerminal("math-expression");
            var parenTerm = new NonTerminal("paren-term");
            var term = new NonTerminal("term");

            var functionCall = new NonTerminal("function-call");
            var parameters = new NonTerminal("params");
            var operatorTerm = new NonTerminal("operator");
            var delete = new NonTerminal("delete");
            var pair = new NonTerminal("pair");
            var hexNumber = new NonTerminal("hex-number");
            #endregion

            #region Place Rules Here
            this.Root = script;

            script.Rule = statements;

            statements.Rule = MakeStarRule(statements, statement);

            statement.Rule = (label + ":") | ((assignment | functionCall | delete | ToTerm("return")) + ";");

            label.Rule = ToTerm("@") + identifier;

            assignmentType.Rule = ToTerm("=") | ":=" | "@=";
            assignment.Rule = lvalue + assignmentType + (pair | expression);

            pair.Rule = ToTerm("(") + expression + "," + expression + ")";

            objectType.Rule = ToTerm("misc") | "animate" | "object" | "enemy";
            arrayAccess.Rule = (objectType + "[" + (simpleExpression | variableRef) + "]") | (ToTerm("map") + "[" + simpleExpression + "," + simpleExpression + "]");

            structAccess.Rule = (arrayAccess | identifier) + "." + identifier;

            lvalue.Rule = structAccess | identifier | variableRef | arrayAccess;

            hexNumber.Rule = ToTerm("$") + number;
            term.Rule = number | identifier | "TRUE" | "FALSE" | "true" | "false" | hexNumber;

            expression.Rule = simpleExpression | labelRef | variableRef;
            simpleExpression.Rule = term | structAccess | parenTerm | mathExpression;

            parenTerm.Rule = ToTerm("(") + simpleExpression + ")";
            mathExpression.Rule = simpleExpression + operatorTerm + simpleExpression;
            labelRef.Rule = (ToTerm("@") + identifier) | ToTerm("*");

            variableRef.Rule = ToTerm("{") + identifier + "}";
            operatorTerm.Rule = ToTerm("+") | "-" | "*" | "/" | "|" | "&";

            functionCall.Rule = identifier + "(" + parameters + ")";
            parameters.Rule = MakeStarRule(parameters, ToTerm(","), expression);

            delete.Rule = ToTerm("delete") + arrayAccess;
            #endregion

            #region Define Keywords
            this.MarkReservedWords("return", "TRUE", "true", "FALSE", "false", "delete");
            #endregion
        }
    }
}
