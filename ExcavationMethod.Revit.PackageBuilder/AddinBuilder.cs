using Autodesk.PackageBuilder;

namespace ExcavationMethod.Revit.PackageBuilder
{
    public class AddinBuilder
    {
        public static void Create(
            string name,
            string path,
            string guid,
            string assembly,
            string fullClassName
            )
        {
            var builder = BuilderUtils.Build<RevitAddInsBuilder>(b =>
            {
                b.AddIn.CreateEntry("Application")
                .Name(name)
                .AddInId(guid)
                .Assembly(assembly)
                .FullClassName(fullClassName)
                .VendorId("AECOM")
                .VendorDescription("AECOM, www.aecom.com");
            });
            var s = builder.Build(path);
        }
    }
}
