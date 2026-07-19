using UnityEngine;

namespace ZelaznaDroga
{
    /// <summary>Builds placeholder meshes and attachment points at runtime.</summary>
    public static class ProceduralModelFactory
    {
        public static void BuildHumanoid(GameObject root, Color cloth)
        {
            AddPart(root, PrimitiveType.Sphere, "Head", new Vector3(0, 1.1f, 0), Vector3.one * .55f, new Color(.45f,.28f,.18f));
            AddPart(root, PrimitiveType.Cube, "Torso", new Vector3(0, .35f, 0), new Vector3(.8f, 1.1f, .45f), cloth);
            AddPart(root, PrimitiveType.Cube, "WeaponSheath", new Vector3(.5f,.2f,0), new Vector3(.12f,.8f,.12f), Color.gray);
        }

        public static void BuildWolf(GameObject root)
        {
            AddPart(root, PrimitiveType.Sphere, "WolfBody", new Vector3(0,.1f,0), new Vector3(1.4f,.8f,2f), new Color(.22f,.22f,.24f));
            AddPart(root, PrimitiveType.Sphere, "WolfHead", new Vector3(0,.35f,.9f), Vector3.one*.65f, new Color(.3f,.3f,.32f));
            AddPart(root, PrimitiveType.Cube, "EarL", new Vector3(-.25f,.75f,.9f), Vector3.one*.2f, new Color(.2f,.2f,.22f));
            AddPart(root, PrimitiveType.Cube, "EarR", new Vector3(.25f,.75f,.9f), Vector3.one*.2f, new Color(.2f,.2f,.22f));
        }

        private static void AddPart(GameObject root, PrimitiveType type, string name, Vector3 position, Vector3 scale, Color color)
        {
            var part = GameObject.CreatePrimitive(type);
            part.name = name; part.transform.SetParent(root.transform, false);
            part.transform.localPosition = position; part.transform.localScale = scale;
            part.GetComponent<Renderer>().material.color = color;
        }
    }
}
