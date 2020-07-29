using System;
using System.Collections.Generic;

namespace Logo
{
    public class Tokens{

        private readonly List<Token> tokens = new List<Token>();
        private int index =-1;

        public void Add(Token token){
            this.tokens.Add(token);
        }

        public Token GetToken(){
            return this.tokens[this.index];
        }

        public Token GetNextToken(){
            this.index++;
            if (this.index < this.tokens.Count){
                return this.tokens[this.index];
            }
            return null;
        }

        public Token GetLastToken(){
            this.index--;
            if (this.index < this.tokens.Count){
                return this.tokens[this.index];
            }
            return null;
        }

        public Token InpectNextToken(){
            if ((this.index + 1) < this.tokens.Count){
                return this.tokens[this.index + 1];
            }
            return null;
        }

        public void ConsumeSpaces(){
            while (this.Next()){
                Token token = this.GetToken();
        
                if (token.Value != " "){
                    this.index -= 1;
                    break;
                }
            }
        }

        public void Reset(){
            this.index = -1;
        }

        public int Index(){
            return this.index;
        }

        public int Count(){
            return this.tokens.Count;
        }

        public bool Next(){
            this.index++;
            return this.HasToken();
        }

        public Tokens Remove(int index){
            this.tokens.RemoveAt(index);
            return this;
        }

        public bool HasToken(){
            if (this.index < this.tokens.Count){
                return true;
            }
            return false;
        }

        public override string ToString(){
            string output = "[\n";
            int idx = 0;
            foreach(Token token in this.tokens){
                string pointer = (idx == this.index) ? "  > " : "    ";
                output += pointer + token + "\n";
                idx++;
            }
            return output + "\n]";
        }
    }
}