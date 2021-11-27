using Machina.Engine;
using Machina.Engine.Cartridges;

namespace Machina.Data
{
    public struct CartridgeBundle
    {
        public readonly GameCartridge cartridge;
        public readonly GameSpecification specification;

        public CartridgeBundle(GameCartridge cartridge, GameSpecification gameSpecification)
        {
            this.cartridge = cartridge;
            this.specification = gameSpecification;
        }
    }
}