// Written by the Swansea Centre for Research in Digital Railways
// 
// in collaboration with Siemens Mobility UK
//
// September 2026
//
// Version 1
//
//-----------------------------
// no liability 
// code may be used freely
//
// File Details 
// --------------
// Filename:   AigConstructor.cs
//
// File Description
// ------------------
// Description:  This file defines the LadderLogicXmlParser class, which parses a ladder logic XML file and converts it into an internal Ladder model 
// composed of rungs and Boolean formulas.
//
// ------------------
//
// GPL-3.0 license
//

//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using System.Xml;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;

namespace SwanLLVerifier.LadderLogic
{
    public class LadderLogicXmlParser
    {
        public static Ladder ParseXML(XmlDocument doc)
        {
            Ladder ladder = new();

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("ladder", "http://www.siemens.com/railautomation/westrace/installation");
            //XmlNodeList equationsListsNodes = doc.DocumentElement.SelectNodes("/ladder:Installation/ladder:EquationsLists", nsmgr);

            XmlNodeList? equationsListsNodes =
            (doc.DocumentElement
            ?? throw new Exception("Document has no root element."))
            .SelectNodes("/ladder:Installation/ladder:EquationsLists", nsmgr);

            if ((equationsListsNodes == null) || (equationsListsNodes.Count != 1))
            {
                throw new Exception("Unexpected value for of EquationsLists nodes.");
            }
            
            XmlNodeList? equationsListNodes = equationsListsNodes.Item(0)!.SelectNodes("ladder:EquationsList", nsmgr);
            if ((equationsListsNodes == null) || (equationsListsNodes.Count != 1))
            {
                throw new Exception("Unexpected value for of EquationsLists nodes.");
            }

            XmlNodeList equationListNodes = equationsListNodes!.Item(0)!.SelectNodes("ladder:EquationList", nsmgr)!;
            if ((equationsListsNodes == null) || (equationsListsNodes.Count != 1))
            {
                throw new Exception("Unexpected value for of EquationsLists nodes.");
            }

            XmlNodeList listItemNodes = equationListNodes.Item(0)!.SelectNodes("ladder:ListItem", nsmgr)!;

            if (listItemNodes == null) {
                throw new Exception("listItemNodes is empty");
            }

            foreach (XmlNode listItemNode in listItemNodes)
            {
                XmlNodeList equationNodes = listItemNode.SelectNodes("ladder:Equation", nsmgr)!
                    ?? throw new Exception("Missing Equation nodes.");
                if ((equationsListsNodes == null) || (equationsListsNodes.Count != 1))
                {
                    throw new Exception("Unexpected value for of EquationsLists nodes.");
                }

                XmlNode equationNode = equationNodes.Item(0)!
                    ?? throw new Exception("Equation node missing.");

                ladder.AddRung(ParseEquation(equationNode, nsmgr));
            }

            return ladder;
        }

        static Rung ParseEquation(XmlNode equationNode, XmlNamespaceManager nsmgr)
        {
            Rung llr = new()
            {
                output = equationNode.SelectSingleNode("ladder:Output", nsmgr)!.InnerText,
                formula = ParseExpr(equationNode.SelectSingleNode("ladder:Expr", nsmgr)!, nsmgr)
            };

            return llr;
        }

        static AbstractFirstOrderFormula ParseExpr(XmlNode exprNode, XmlNamespaceManager nsmgr)
        {
            string op = exprNode.SelectSingleNode("ladder:Op", nsmgr)!.InnerText;
            XmlNodeList? exprItemNodes = exprNode.SelectNodes("ladder:ExprItem", nsmgr);

            if (exprItemNodes == null)
            {
            throw new InvalidOperationException("Missing ExprItem nodes.");
            }

            // Parse each exprItem
            List<AbstractFirstOrderFormula> parsedExprItems = new();
            foreach (XmlNode exprItem in exprItemNodes)
            {
                parsedExprItems.Add(ParseExprItem(exprItem, nsmgr));
            }

            AbstractFirstOrderFormula? result = null;

            switch (op)
            {
                case "AND":
                    result = MakeAnd(parsedExprItems);
                    break;
                case "OR":
                    result = MakeOr(parsedExprItems);
                    break;
                default:
                    throw new Exception("Unexpected expression Op: " + op);
            }

            return result;
        }

        static AbstractFirstOrderFormula ParseExprItem(XmlNode expressionNode, XmlNamespaceManager nsmgr)
        {
            // These can have eiher a Input element or an ExprItem element
            if (expressionNode.ChildNodes.Count != 1)
            {
                throw new Exception("Unexpected number of children of ExprItem nodes: " + expressionNode.ChildNodes.Count);
            }

            XmlNode? child = expressionNode.ChildNodes.Item(0);

            if (child == null)
            {
                throw new InvalidOperationException("Expression node has no children.");
            }

            string nameOfChildElement = child.Name;

            switch (nameOfChildElement)
            {
                case "Input":
                    return ParseInput(child, nsmgr);
                case "Expr":
                    return ParseExpr(child, nsmgr);
                default:
                    throw new Exception("Unexpected child of ExprItem: " + nameOfChildElement);
            }
        }

        static AbstractFirstOrderFormula ParseInput(XmlNode inputNode, XmlNamespaceManager nsmgr)
        {
            XmlNode? name = inputNode.SelectSingleNode("ladder:Name", nsmgr);
            XmlNode? negated = inputNode.SelectSingleNode("ladder:Negate", nsmgr);

            if (name == null)
            {
            throw new InvalidOperationException("Input node does not contain a Name element.");
            }
            
            if (negated != null && negated.InnerText.Equals("true"))
            {
                return MakeNegatedVar(name.InnerText);
            }
            else
            {
                return MakeVar(name.InnerText);
            }
        }

    }
}
