using System.Text;

namespace Machina.Data
{
    public struct ModifierKeys
    {
        private readonly bool control;
        private readonly bool alt;
        private readonly bool shift;

        public static ModifierKeys NoModifiers = new ModifierKeys(false, false, false);

        public ModifierKeys(bool control, bool alt, bool shift)
        {
            this.control = control;
            this.alt = alt;
            this.shift = shift;
        }

        public ModifierKeys(int encodedInt) : this()
        {
            this.control = (encodedInt & 1) == 1;
            this.alt = (encodedInt & 2) == 2;
            this.shift = (encodedInt & 4) == 4;
        }

        public static bool operator ==(ModifierKeys a, ModifierKeys b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ModifierKeys a, ModifierKeys b)
        {
            return !a.Equals(b);
        }

        public bool None => !this.control && !this.alt && !this.shift;
        public bool Control => this.control && !this.alt && !this.shift;
        public bool ControlAlt => this.control && this.alt && !this.shift;
        public bool Alt => !this.control && this.alt && !this.shift;
        public bool Shift => !this.control && !this.alt && this.shift;
        public bool AltShift => !this.control && this.alt && this.shift;
        public bool ControlShift => this.control && !this.alt && this.shift;
        public bool ControlAltShift => this.control && this.alt && this.shift;

        public int EncodedInt =>
            (Bool2Int(this.control) << 2) | (Bool2Int(this.alt) << 1) | (Bool2Int(this.shift) << 0);

        public int Bool2Int(bool b)
        {
            return b ? 1 : 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (this.control)
            {
                sb.Append('^');
            }

            if (this.shift)
            {
                sb.Append('+');
            }

            if (this.alt)
            {
                sb.Append('~');
            }

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is ModifierKeys other)
            {
                return GetHashCode() == obj.GetHashCode();
            }

            return false;
        }

        public override int GetHashCode()
        {
            var shift = this.shift ? 1 << 0 : 0;
            var ctrl = this.control ? 1 << 1 : 0;
            var alt = this.alt ? 1 << 2 : 0;

            return ctrl | alt | shift;
        }
    }
}
