using System;
using System.Collections.Generic;
using System.Text;

namespace Logo
{
    class Compiler{
        private Tokens tokens;
        private Dictionary<string, Action<Tokens, FuncArgs>> functions = new Dictionary<string, Action<Tokens, FuncArgs>>();
        private int width = 700;
        private int height = 700;
        private double angle = 270;
        private double x1 = 350;
        private double y1 = 350;
        private string strokeColor = "#000";
        private bool penState = true;
        private StringBuilder output = new StringBuilder();
        public Compiler(Tokens tokens){
            this.tokens = tokens;
            this.functions.Add("setcanvas", FUNC_SETCANVAS);
            this.functions.Add("home", FUNC_HOME);
            this.functions.Add("penup", FUNC_PENUP);
            this.functions.Add("pu", FUNC_PENUP);
            this.functions.Add("pendown", FUNC_PENDOWN);
            this.functions.Add("pd", FUNC_PENDOWN);
            this.functions.Add("setpencolor", FUNC_SETPENCOLOR);
            this.functions.Add("setpc", FUNC_SETPENCOLOR);
            this.functions.Add("randpencolor", FUNC_RANDPENCOLOR);
            this.functions.Add("randpc", FUNC_RANDPENCOLOR);
            this.functions.Add("randposition", FUNC_RANDPOSITION);
            this.functions.Add("randps", FUNC_RANDPOSITION);
            this.functions.Add("forward", FUNC_FORWARD);
            this.functions.Add("fd", FUNC_FORWARD);
            this.functions.Add("right", FUNC_RIGHT);
            this.functions.Add("rt", FUNC_RIGHT);
            this.functions.Add("left", FUNC_LEFT);
            this.functions.Add("lt", FUNC_LEFT);
            this.functions.Add("print", FUNC_PRINT);
            this.functions.Add("let", FUNC_LET);
            this.functions.Add("repeat", FUNC_REPEAT);
            this.functions.Add("func", FUNC_FUNCTION);
        }
        public string Execute(){
            output.AppendFormat("<svg width='{0}' height='{1}'>", this.width, this.height);
            this.Compile(this.tokens, new FuncArgs());
            output.AppendLine("</svg>");
            return this.output.ToString();
        }
        private void Compile(Tokens tokens, FuncArgs funcArgs){
            tokens.ConsumeSpaces();

            Token token = tokens.InpectNextToken();

            if(token !=null){
                string value = token.Value.ToLower();

                if(this.functions.ContainsKey(value)){
                    this.functions[value](tokens, funcArgs);
                }else{
                    throw new SyntaxError(string.Format("{0} is not defined near line {1}.", token.Value, token.Line));
                }
            }
        }
        private void Next(Tokens tokens, FuncArgs funcArgs){
            tokens.ConsumeSpaces();

            if(tokens.HasToken()){
                this.Compile(tokens, funcArgs);
            }
        }
        private string GetFuncArg(string argName, FuncArgs funcArgs){
            if(argName.Substring(0,1) == ":"){
                if(funcArgs.ContainsKey(argName)){
                    return funcArgs[argName];
                }else{
                    throw new SyntaxError(string.Format("Undefined variable {0}.", argName));
                }
            }
            return argName;
        }
        private string Expression(Tokens tokens, FuncArgs funcArgs){

            double val1 = 0;
            double val2 = 0;

            Token openParenToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token val1Token = tokens.GetNextToken();

            val1 = Convert.ToDouble(this.GetFuncArg(val1Token.Value, funcArgs));

            tokens.ConsumeSpaces();

            Token opToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            if(tokens.InpectNextToken().Value == "("){
                val2 = Convert.ToDouble(this.Expression(tokens, funcArgs));
            }else{
                Token val2Token = tokens.GetNextToken();
                val2 = Convert.ToDouble(this.GetFuncArg(val2Token.Value, funcArgs));
            }

            tokens.ConsumeSpaces();

            Token closeParenToken = tokens.GetNextToken();

            double result = 0;

            switch(opToken.Value){
                case "+":
                    result = val1 + val2;
                    break;
                case "-":
                    result = val1 - val2;
                    break;
                case "/":
                    result = val1 / val2;
                    break;
                case "*":
                    result = val1 * val2;
                    break;
            }

            return result.ToString();
        }
        private void FUNC_SETCANVAS(Tokens tokens, FuncArgs funcArgs){
            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcWidthArgToken = tokens.GetNextToken();

            if(funcWidthArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 3 arguments.", funcNameToken.Line,  funcNameToken.Value.ToUpper()));
            }

            try{
                this.width = Convert.ToInt32(this.GetFuncArg(funcWidthArgToken.Value, funcArgs));
            }
            catch(FormatException e){
                throw new SyntaxError(string.Format("Error on line {0}. {1} function expects arg1 to be a number. {2}", funcWidthArgToken.Line, funcNameToken.Value.ToUpper(), e.Message));
            }

            tokens.ConsumeSpaces();

            Token funcHeightArgToken = tokens.GetNextToken();

            if(funcHeightArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 3 arguments.", funcWidthArgToken.Line,  funcNameToken.Value.ToUpper()));
            }

            try{
                this.height = Convert.ToInt32(this.GetFuncArg(funcHeightArgToken.Value, funcArgs));
            }
            catch(FormatException e){
                throw new SyntaxError(string.Format("Error on line {0}. {1} function expects arg2 to be a number. {2}", funcHeightArgToken.Line, funcNameToken.Value.ToUpper(), e.Message));
            }

            tokens.ConsumeSpaces();

            Token funcBgColorArgToken = tokens.GetNextToken();

            if(funcBgColorArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 3 arguments.", funcHeightArgToken.Line,  funcNameToken.Value.ToUpper()));
            }

            string bgColor = funcBgColorArgToken.Value;

            this.output.Clear();
            this.output.AppendFormat("<svg width='{0}' height='{1}' style='background-color:{2}'>", this.width, this.height, bgColor);

            this.x1 = this.width / 2;
            this.y1 = this.height / 2;
            this.angle = 270;
            this.strokeColor = "#000";
            this.penState = true;

            this.Next(tokens, funcArgs);
        }
        private void FUNC_HOME(Tokens tokens, FuncArgs funcArgs){

            Token funcNameToken = tokens.GetNextToken();

            this.x1 = 350;
            this.y1 = 350;

            this.Next(tokens, funcArgs);
        }
        private void FUNC_PENUP(Tokens tokens, FuncArgs funcArgs){
            tokens.GetNextToken();
            this.penState = false;

            this.Next(tokens, funcArgs);
        }
        private void FUNC_PENDOWN(Tokens tokens, FuncArgs funcArgs){
            tokens.GetNextToken();
            this.penState = true;

            this.Next(tokens, funcArgs);
        }
        private void FUNC_SETPENCOLOR(Tokens tokens, FuncArgs funcArgs){

            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcArgToken = tokens.GetNextToken();

            if(funcArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 1 argument.", funcNameToken.Line,  funcNameToken.Value.ToUpper()));
            }

            this.strokeColor = this.GetFuncArg(funcArgToken.Value, funcArgs);

            this.Next(tokens, funcArgs);
        }
        private void FUNC_RANDPENCOLOR(Tokens tokens, FuncArgs funcArgs){
            Token funcNameToken = tokens.GetNextToken();

            string[] colors = new string[10]{ "#CD5C5C", "#FF69B4", "#FFA500", "#FFD700", "#DDA0DD", "#7B68EE", "#ADFF2F", "#9ACD32", "#00FFFF", "#87CEFA" };

            Random rand = new Random();
            int randNum = rand.Next(0,9);
            this.strokeColor = colors[randNum];

            this.Next(tokens, funcArgs);
        }

        private void FUNC_RANDPOSITION(Tokens tokens, FuncArgs funcArgs){

            Token funcNameToken = tokens.GetNextToken();

            Random rand1 = new Random();
            this.x1 = rand1.Next(this.width);

            this.y1 = rand1.Next(this.height);

            this.Next(tokens, funcArgs);
        }
        private void FUNC_FORWARD(Tokens tokens, FuncArgs funcArgs){
            this.FUNC_DIRECTION(tokens, 0, funcArgs);
        }
        private void FUNC_DIRECTION(Tokens tokens, int dir, FuncArgs funcArgs){

            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcArgToken = tokens.GetNextToken();

            if(funcArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 1 argument.", funcNameToken.Line, funcNameToken.Value.ToUpper()));
            }

            int distance = 0;

            try{
                distance = Convert.ToInt32(this.GetFuncArg(funcArgToken.Value, funcArgs));
            }
            catch(FormatException e){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects a number. {2}", funcNameToken.Line, funcNameToken.Value.ToUpper(), e.Message));
            }

            double tmpAngle = this.angle;

            this.angle += dir;

            double x2 = distance * Math.Cos(this.angle * Math.PI / 180) + this.x1;
            double y2 = distance * Math.Sin(this.angle * Math.PI / 180) + this.y1;

            if(this.penState){
                this.output.AppendFormat("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' style='stroke:{4};stroke-width:2' />",
                    this.x1,
                    this.y1, 
                    x2,
                    y2, 
                    this.strokeColor
                );
            }

            this.angle = tmpAngle;

            this.x1 = x2;
            this.y1 = y2;

            this.Next(tokens, funcArgs);
        }
        private void FUNC_RIGHT(Tokens tokens, FuncArgs funcArgs){

            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcArgToken = tokens.GetNextToken();

            if(funcArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 1 argument.", funcNameToken.Line,  funcNameToken.Value.ToUpper()));
            }

            double angle = 0;

            try{
                angle = Convert.ToInt32(this.GetFuncArg(funcArgToken.Value, funcArgs));
            }
            catch(FormatException e){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects a number. {2}", funcNameToken.Line,  funcNameToken.Value.ToUpper(), e.Message));
            }

            this.angle += angle;

            this.Next(tokens, funcArgs);
        }
        private void FUNC_LEFT(Tokens tokens, FuncArgs funcArgs){
            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcArgToken = tokens.GetNextToken();

            if(funcArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 1 argument.", funcNameToken.Line,  funcNameToken.Value.ToUpper()));
            }

            double angle = 0;

            try{
                angle = Convert.ToInt32(this.GetFuncArg(funcArgToken.Value, funcArgs));
            }
            catch(FormatException e){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects a number. {2}", funcNameToken.Line,  funcNameToken.Value.ToUpper(), e.Message));
            }

            this.angle -= angle;

            this.Next(tokens, funcArgs);
        }
        private void FUNC_PRINT(Tokens tokens, FuncArgs funcArgs){
            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token varNameToken = tokens.GetNextToken();

            if(varNameToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 1 argument.", funcNameToken.Line,  funcNameToken.Value.ToUpper()));
            }

            Console.WriteLine(this.GetFuncArg(varNameToken.Value, funcArgs));
        }
        private void FUNC_LET(Tokens tokens, FuncArgs funcArgs){
            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token varNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token equalToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            string val = string.Empty;

            if(tokens.InpectNextToken().Value == "("){
                val = this.Expression(tokens, funcArgs);
            }else{
                val = this.GetFuncArg(tokens.GetNextToken().Value, funcArgs);
            }

            string varName = varNameToken.Value;

            if(funcArgs.ContainsKey(varName)){
                funcArgs[varName] = val;
            }else{
                funcArgs.Add(varName, val);
            }
            
            this.Next(tokens, funcArgs);
        }
        private void FUNC_REPEAT(Tokens tokens, FuncArgs funcArgs){
            FuncArgs scopedFuncArgs = new FuncArgs(funcArgs);
            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcArgToken = tokens.GetNextToken();

            if(funcArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 1 argument.", funcNameToken.Line,  funcNameToken.Value.ToUpper()));
            }

            int counter = 0;

            try{
                counter = Convert.ToInt32(this.GetFuncArg(funcArgToken.Value, funcArgs));
            }
            catch(FormatException e){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects a number. {2}", funcNameToken.Line,  funcNameToken.Value.ToUpper(), e.Message));
            }

            tokens.ConsumeSpaces();

            Token leftBracket = tokens.GetNextToken();

            if(leftBracket == null){
                throw new SyntaxError(string.Format("Unexpected identifier near line {0}.", funcArgToken.Line));
            }

            if(leftBracket.Value != "["){
                throw new SyntaxError(string.Format("Unexpected identifier on line {0}", leftBracket.Line));
            }

            Tokens repeatStmTokens = new Tokens();

            int openBracketCount = 1;

            while(tokens.Next()){
                Token token = tokens.GetToken();

                if(token.Value == "]"){
                    if(openBracketCount == 1){
                        break;
                    }else{
                        openBracketCount--;
                    }
                }

                repeatStmTokens.Add(token);

                if(token.Value == "["){
                    openBracketCount++; 
                }
            } 

            if(tokens.Index() == tokens.Count()){
                Token lastToken = tokens.GetLastToken();
                throw new SyntaxError(string.Format("Unexpected end of input near line {0}.", lastToken.Line));
            }

            tokens.ConsumeSpaces();

            string indexName = "";

            if(tokens.InpectNextToken() != null){
                if(tokens.InpectNextToken().Value.Substring(0, 1) == ":"){
                    indexName = tokens.GetNextToken().Value;
                }
            }

            for(int i=0; i < counter; i++){
                if(!string.IsNullOrEmpty(indexName)){
                    scopedFuncArgs.Add(indexName, i.ToString());
                }
                repeatStmTokens.Reset();
                this.Compile(repeatStmTokens, scopedFuncArgs);
                scopedFuncArgs.Remove(indexName);
            }

            this.Next(tokens, funcArgs);
        }
        private void FUNC_FUNCTION(Tokens tokens, FuncArgs funcArgs){
            Token funcToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcNameToken = tokens.GetNextToken();

            if(funcNameToken == null){
                throw new SyntaxError(string.Format("Function name required near line {0}.", funcToken.Line));
            }

            tokens.ConsumeSpaces();

            Dictionary<string, Token> funcArgList = new Dictionary<string, Token>();

            if(tokens.InpectNextToken().Value != "["){

                while(tokens.Next()){

                    Token token = tokens.GetToken();

                    if(token.Value.Substring(0, 1) != ":"){
                        throw new SyntaxError(string.Format("Unexpected token {0} on line {1}.", token.Value, token.Line));
                    }
                    
                    if(funcArgList.ContainsKey(token.Value)){
                        Token varToken = funcArgList[token.Value];
                        throw new SyntaxError(string.Format("Variable {0} has already been defined on line {1}.", token.Value, varToken.Line));
                    }

                    funcArgList.Add(token.Value, token);

                    tokens.ConsumeSpaces();

                    if(tokens.InpectNextToken().Value == "["){
                        break;
                    }
                } 
            }

            tokens.ConsumeSpaces();

            Tokens funcStmTokens = new Tokens();

            int openBracketCount = 0;

            while(tokens.Next()){

                Token token = tokens.GetToken();

                funcStmTokens.Add(token);

                if(token.Value == "]"){
                    openBracketCount--;
                    if(openBracketCount == 0){
                        break;
                    }
                }
                if(token.Value == "["){
                    openBracketCount++; 
                }

                if(token.Value.ToLower() == "func"){
                    throw new SyntaxError(string.Format("Func not allowed ", token.Value, token.Line));
                }
            } 

            if(tokens.Index() == tokens.Count()){
                Token lastToken = tokens.GetLastToken();
                throw new SyntaxError(string.Format("Unexpected end of input near line {0}.", lastToken.Line));
            }

            tokens.ConsumeSpaces();

            funcStmTokens.Remove(0).Remove(funcStmTokens.Count()-1);

            tokens.ConsumeSpaces();

            this.functions.Add(funcNameToken.Value.ToLower(), (scopedTokens, scopedFuncArgs) => {
                Token localFuncNameToken = scopedTokens.GetNextToken();

                FuncArgs localFuncArgs = new FuncArgs();

                foreach(KeyValuePair<string, Token> pair in funcArgList){
                    scopedTokens.ConsumeSpaces();
                    string argName = pair.Value.Value;
                    Token argValueToken = scopedTokens.GetNextToken();

                    if(argValueToken != null){ 
                        string value = argValueToken.Value;

                        if(value.Substring(0,1) == ":"){
                            if(scopedFuncArgs.ContainsKey(value)){
                                value = scopedFuncArgs[value];
                            }
                        }
                        localFuncArgs.Add(argName, value);
                    }else{
                        throw new SyntaxError(string.Format("Function {0} expects {1} argument(s) near line {2}", localFuncNameToken.Value, funcArgList.Count, localFuncNameToken.Line));
                    }
                }

                this.Compile(funcStmTokens, localFuncArgs);
                funcStmTokens.Reset();
                this.Next(scopedTokens, scopedFuncArgs); 
            });

            this.Next(tokens, funcArgs);
        }
    }
    class FuncArgs : Dictionary<string, string>{

        public FuncArgs(){}
        public FuncArgs(FuncArgs args){
            foreach(KeyValuePair<string, string> pair in args){
                this.Add(pair.Key, pair.Value);
            }
        }

        public override string ToString(){
            string output = "[\n";
            foreach(KeyValuePair<string, string> pair in this){
                output += string.Format("    {{ {0} : {1} }}\n", pair.Key, pair.Value);
            }
            return output + "]";
        }
    }
}