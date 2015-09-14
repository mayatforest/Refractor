using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Refractor.Common;
using JS = Microsoft.JScript;

namespace Refractor.Plugins.JScript
{
    public class JSMethodsGb : BaseJSGraphBuilder
    {
        public override void Translate()
        {
            FirstPass();
            SecondPass();
        }

        private Dictionary<string, JS.AST> _methodInfos = new Dictionary<string, JS.AST>();
        private List<string> _names = new List<string>();
        private List<string> _overloads = new List<string>();

        private List<JSMethodItem> GetMethods()
        {
            if (_activeItem is JSClassItem)
            {
                return (_activeItem as JSClassItem).Methods;
            }
            else if (_activeItem is JSFileItem)
            {
                return (_activeItem as JSFileItem).Methods;
            }
            return null;
        }

        private void FirstPass()
        {
            foreach (JSMethodItem methodItem in GetMethods())
            {
                if (CheckWorker()) break;

                JS.AST mi = methodItem.MethodInfo;

                // Get the type, itemPath and parameter list from the ToString override.
                string id = methodItem.GetID();

                // Add the graph node with a link back to the method.
                AddNode(methodItem.Name, methodItem.Name, _sharedOptions.MethodColor, methodItem); 
                //child = (Node)_graph.AddNode(methodItem.Name);
                //child.UserData = methodItem;

                // Hang onto the method and the corresponding graph node.
                _methodInfos.Add(methodItem.Name, mi); //id, mi);

                // Make note of overloaded methods.
                if (_names.Contains(methodItem.Name)) 
                    _overloads.Add(methodItem.Name);
                else 
                    _names.Add(methodItem.Name);
            }
        }

        private string _methodNodeId;

        private void SecondPass()
        {
            // For each method in the class in turn.
            foreach (KeyValuePair<string, JS.AST> pair in _methodInfos)
            {
                if (CheckWorker()) break;

                object methodNode = _addedNodes[pair.Key];

                // Append caption for overloaded methods.
                if (_overloads.Contains(pair.Key))
                {
                    AppendNodeCaption(methodNode, "\n(overloaded)");
                }

                UpdateNodeColor(methodNode, Color.LightGoldenrodYellow);

                _methodNodeId = pair.Key;

                //_logView.LogStr("----------------------------------");
                //_logView.LogStr("Method:" + pair.Key);
                //_logView.LogStr("----------------------------------");
                ParseAst(pair.Value, string.Empty);
            }
        }

        private void ParseAstList(ArrayList astItems, string sp)
        {
            foreach (JS.AST ast in astItems)
            {
                if (CheckWorker()) break;
                ParseAst(ast, sp);
            }
        }

        private void ParseAst(JS.AST ast, string sp)
        {
            if (ast == null) return;
            if (CheckWorker()) return;

            //_logView.LogStr("JSM->" + sp + ast.ToString() + "\t\t" + ast.GetType().Name);

            if (ast is JS.FunctionDeclaration)
            {
                JS.Function func = ast as JS.Function;
                ParseAstList(func.func_obj.body.elems, sp + "  ");
            }
            else if (ast is JS.Assign)
            {
                JS.Assign ass = ast as JS.Assign;

                ParseAst(ass.left, sp + "l ");
                ParseAst(ass.right, sp + "r ");
            }
            else if (ast is JS.Binary)
            {
                JS.Binary bin = ast as JS.Binary;

                string[] parts = bin.ToString().Split('.');
                if (parts.Length > 1)
                {
                    if (parts[parts.Length - 2] == "This")
                    {
                        string calledId = parts[parts.Length - 1] + "()";

                        //dup
                        // If we have a method of this name in this file/class
                        if (_addedNodes.ContainsKey(calledId))
                        {
                            // Create an edge.
                            // A definite functional assignment link.
                            AddEdge(_methodNodeId, calledId, EdgeStyle.NormalArrow, Color.Aqua);
                        }
                        else
                        {
                            // It's a call to a method outside this class/file.
                            //
                            //_logView.LogStr("skipped assign ref -->" + _methodNodeId + " --------> " + calledId);
                            //
                        }
                    }
                    else if (parts[parts.Length - 2] == "self")
                    {
                        string calledId = parts[parts.Length - 1] + "()";

                        //dup
                        // If we have a method of this name in this file/class
                        if (_addedNodes.ContainsKey(calledId))
                        {
                            // Get the graph node that we're linking to.
                            //Node calledNode = _addedNodes[calledId];

                            // Create an edge.
                            // A definite functional assignment link.
                            AddEdge(_methodNodeId, calledId, EdgeStyle.NormalArrow, Color.Aqua);
                        }
                        else
                        {
                            // It's a call to a method outside this class/file.
                            _logView.LogStr("skipped assign ref -->" + _methodNodeId + " --------> " + calledId);
                        }
                    }
                }
            }
            else if (ast is JS.Expression)
            {
                JS.Expression expr = ast as JS.Expression;
                ParseAstList(expr.exprs, sp + "  ");
            }
            else if (ast is JS.FunctionExpression)
            {
                JS.FunctionExpression expr = ast as JS.FunctionExpression;
                ParseAstList(expr.func_obj.body.elems, sp + "  ");
            }
            else if (ast is JS.For)
            {
                JS.For fr = ast as JS.For;
                ParseAst(fr.stms, sp + "  ");
            }
            else if (ast is JS.If)
            {
                JS.If iff = ast as JS.If;
                ParseAst(iff.false_stm, sp + "f  ");
                ParseAst(iff.true_stm, sp + "t  ");
            }
            else if (ast is JS.Block)
            {
                JS.Block block = ast as JS.Block;
                ParseAstList(block.elems, sp + "  ");
            }
            else if (ast is JS.VariableStatement)
            {
                JS.VariableStatement var = ast as JS.VariableStatement;
                //var.
                ParseAstList(var.var_decls, sp + "  ");
            }
            else if (ast is JS.Return)
            {
                JS.Return ret = ast as JS.Return;

                ParseAst(ret.expression, sp + "  ");
            }
            else if (ast is JS.VariableDeclaration)
            {
                JS.VariableDeclaration var = ast as JS.VariableDeclaration;

                Microsoft.JScript.New newval = var.val as Microsoft.JScript.New;

                if (newval != null && newval.exp != null)
                {
                    //_logView.LogStr("new:" + newval.exp.ToString());
                    string[] parts = newval.exp.ToString().Split('.');
                    if (parts.Length > 0)
                    {
                        string calledId = parts[parts.Length - 1] + "()";

                        // If we have a method of this name in this file/class, then 
                        // we have a possible constructor link.
                        if (_addedNodes.ContainsKey(calledId))
                        {
                            // Create an edge.
                            AddEdge(_methodNodeId, calledId, EdgeStyle.NormalArrow, Color.Green);
                        }
                    }
                }
                else
                {
                    if (var.val != null)
                    {
                        string valStr = var.val.ToString();
                        string[] parts = valStr.Split('.');

                        if (parts.Length > 1 && parts[0] == "self")
                        {
                            // dup..

                            string calledId = parts[parts.Length - 1] + "()";

                            // If we have a method of this name in this file/class, then 
                            // we have a possible constructor link.
                            if (_addedNodes.ContainsKey(calledId))
                            {
                                // Create an edge.
                                AddEdge(_methodNodeId, calledId, EdgeStyle.NormalArrow, Color.Green);
                            }
                        }
                    }
                }

                //ParseAstList(var.var_decls, sp + "  ");
            }
            else if (ast is JS.Call)
            {
                JS.Call call = ast as JS.Call;
                string[] parts = call.ToString().Split(' ');
                string[] bits = parts[0].Split('.');
                string calledId = bits[bits.Length - 1] + "()";

                bool methodInThisClass = true;
                if (bits.Length > 1)
                {
                    if ((bits[bits.Length - 2] != "This") &&
                        (bits[bits.Length - 2] != "self"))
                    {
                        methodInThisClass = false;
                    }
                }

                // If we have a method of this name in this file/class
                if (_addedNodes.ContainsKey(calledId))
                {
                    // Create an edge.
                    if (methodInThisClass)
                    {
                        // A definite link.
                        AddEdge(_methodNodeId, calledId, EdgeStyle.NormalArrow, Color.Black);
                    }
                    else
                    {
                        // A tentative link.
                        AddEdge(_methodNodeId, calledId, EdgeStyle.NormalArrow, Color.Gray);
                    }
                }
                else
                {
                    // It's a call to a method outside this class/file.                    
                    //
                    //_logView.LogStr("skipped -------->" + _methodNodeId + " --------> " + parts[0]);
                    //
                }
            }
        }

    }
}
