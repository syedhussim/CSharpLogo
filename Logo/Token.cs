namespace Logo{

    public class Token{

        public readonly int Line;
        public readonly string Value;
        public Token(int lineNumber, string tokenValue){
            this.Line = lineNumber + 1;
            this.Value = tokenValue;
        }

        public override string ToString(){
            return string.Format("{{ line : {0}, token : {1} }}", this.Line, this.Value);
        }
    }
}