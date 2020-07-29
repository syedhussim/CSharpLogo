using System;
using System.Collections.Generic;

namespace Logo
{
    public class Lexer{

        public static Tokens Parse(string str){

            List<string> symbols = new List<string>();
            symbols.Add(" ");
            symbols.Add("[");
            symbols.Add("]");

            string[] lines = str.Split("\n");

            string strToken = "";
            Token token;
            Tokens tokens = new Tokens();

            for(int i=0; i < lines.Length; i++){
                strToken = "";
                string line = lines[i].Trim();


                for(int j=0; j < line.Length; j++){
                    string ch = line[j].ToString();

                    if(symbols.Contains(ch)){
                        if(strToken.Length > 0){
                            token = new Token(i, strToken);
                            tokens.Add(token);
                        }

                        token = new Token(i, ch);
                        tokens.Add(token);

                        strToken = "";
                        continue;
                    }

                    strToken += ch.ToString();
                }

                if(strToken.Length > 0){
                    token = new Token(i, strToken);
                    tokens.Add(token);
                }
            }

            return tokens;
        }
    }
}