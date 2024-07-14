using LDtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HyperDigger
{
    static class EntityFactory
    {
        public static GameObject CreateEntity(Container container, TilemapWorld world, EntityInstance entity)
        {
            // Process entity fields.
            var type = "";
            foreach (var field in entity.FieldInstances)
            {
                if (field._Type == "String" && field._Identifier == "Type") type = field._Value.ToString();
            }
            if (type.Length != 0)
            {
                try
                {
                    Type eType = Type.GetType("HyperDigger.Entities." + type);
                    object instance = Activator.CreateInstance(eType, container, world, entity);
                    return instance as GameObject;
                }
                catch (Exception ex) { }
            }
            
            return null;
        }
    }
}
