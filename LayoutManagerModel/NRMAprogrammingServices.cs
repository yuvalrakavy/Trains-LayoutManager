namespace LayoutManager.Model {
    public class ReadCVresult {
        public LayoutActionResult Result { get; }
        public int CV { get; }
        public byte Value { get; }

        public ReadCVresult(LayoutActionResult programmingResult, int cv, byte value) {
            this.Result = programmingResult;
            this.CV = cv;
            this.Value = value;
        }
    }
}
