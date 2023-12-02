using PulsarModLoader;

namespace Sensor_Dish_Targeting
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "18107";

        public override string ShortDescription => "Allows for easier collection of scrap when ships are around";

        public override string Name => "Sensor Dish Targeting";

        public override string ModID => "sensordishtargeting";

        public override string HarmonyIdentifier()
        {
            return "id107.sensordishtargeting";
        }
    }
}
