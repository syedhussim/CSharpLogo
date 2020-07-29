using System;
using System.Collections.Generic;
using System.Text;

namespace Logo
{
    class Compiler{

        private Tokens tokens;
        private Dictionary<string, Action<Tokens, FuncArgs>> functions = new Dictionary<string, Action<Tokens, FuncArgs>>();
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
            this.functions.Add("pendown", FUNC_PENDOWN);
            this.functions.Add("setpencolor", FUNC_SETPENCOLOR);
            this.functions.Add("forward", FUNC_FORWARD);
            this.functions.Add("right", FUNC_RIGHT);
            this.functions.Add("left", FUNC_LEFT);
            this.functions.Add("repeat", FUNC_REPEAT);
            this.functions.Add("func", FUNC_FUNCTION);
        }

        public string Execute(){
            output.AppendFormat("<svg width='700' height='700'>");
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
            if(funcArgs.ContainsKey(argName)){
                return funcArgs[argName];
            }
            return argName;
        }

        private void FUNC_SETCANVAS(Tokens tokens, FuncArgs funcArgs){
            Token funcNameToken = tokens.GetNextToken();

            tokens.ConsumeSpaces();

            Token funcWidthArgToken = tokens.GetNextToken();

            if(funcWidthArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 3 arguments.", funcNameToken.Line,  funcNameToken.Value.ToUpper()));
            }

            int width = 0;

            try{
                width = Convert.ToInt32(this.GetFuncArg(funcWidthArgToken.Value, funcArgs));
            }
            catch(FormatException e){
                throw new SyntaxError(string.Format("Error on line {0}. {1} function expects arg1 to be a number. {2}", funcWidthArgToken.Line, funcNameToken.Value.ToUpper(), e.Message));
            }

            tokens.ConsumeSpaces();

            Token funcHeightArgToken = tokens.GetNextToken();

            if(funcHeightArgToken == null){
                throw new SyntaxError(string.Format("Error near line {0}. {1} function expects 3 arguments.", funcWidthArgToken.Line,  funcNameToken.Value.ToUpper()));
            }

            int height = 0;

            try{
                height = Convert.ToInt32(this.GetFuncArg(funcHeightArgToken.Value, funcArgs));
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
            this.output.AppendFormat("<svg width='{0}' height='{1}' style='background-color:{2}'>", width, height, bgColor);

            this.x1 = width / 2;
            this.y1 = width / 2;
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
        private void FUNC_REPEAT(Tokens tokens, FuncArgs funcArgs){
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
                    funcArgs.Add(indexName, i.ToString());
                }
                repeatStmTokens.Reset();
                this.Compile(repeatStmTokens, funcArgs);
                funcArgs.Remove(indexName);
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

                this.Next(scopedTokens, scopedFuncArgs); 
            });

            this.Next(tokens, funcArgs);
        }
    }
    class FuncArgs : Dictionary<string, string>{

        public override string ToString(){
            string output = "[\n";
            foreach(KeyValuePair<string, string> pair in this){
                output += string.Format("    {{ {0} : {1} }}\n", pair.Key, pair.Value);
            }
            return output + "]";
        }
    }
}