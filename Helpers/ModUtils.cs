using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Core;
using Terraria.ModLoader;

namespace UICustomizer.Helpers
{
    public static class ModUtils
    {
        /// <summary>
        /// Gets an instance of a Mod based on the provided type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static Mod GetModInstance(Type type)
        {
            var modType = AssemblyManager.GetLoadableTypes(type.Assembly)
                .FirstOrDefault(t => t.IsSubclassOf(typeof(Mod)) && !t.IsAbstract, null);
            if (modType == null)
                return null;

            // Imagine this as ModContent.GetInstance<modType>()
            var method = typeof(ModContent)
                .GetMethod(nameof(ModContent.GetInstance))
                ?.GetGenericMethodDefinition()
                ?.MakeGenericMethod(modType);
            var result = method?.Invoke(null, null);

            if (result is not Mod instance)
                throw new InvalidCastException($"{modType.FullName} is not a Mod or could not be instantiated via ModContent.GetInstance<T>()");

            return instance;
        }
    }
}
