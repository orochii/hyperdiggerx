using LDtk;
using Microsoft.Xna.Framework;
using System;
using static Assimp.Metadata;
using System.ComponentModel;

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
                catch (Exception ex) {
                    System.Console.WriteLine(ex.StackTrace);
                }
            }
            
            return null;
        }

        public static GameObject CreateCardEntity(string type, GameObject owner, Vector2 position)
        {
            if (type.Length != 0)
            {
                try
                {
                    Type eType = Type.GetType("HyperDigger.CardTypes." + type);
                    object instance = Activator.CreateInstance(eType, owner.Container, owner, position);
                    return instance as GameObject;
                }
                catch (Exception ex) {
                    System.Console.WriteLine(ex.StackTrace);
                }
            }

            return null;
        }
    }
}
