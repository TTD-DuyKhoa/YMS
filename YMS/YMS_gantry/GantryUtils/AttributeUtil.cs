using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using System.Threading.Tasks ;

namespace YMS_gantry
{
  class UtilAttribute<AttributeType> where AttributeType : NameAttribute
  {
    public static SuperType CreateFrom<SuperType>( string className ) where SuperType : class
    {
      var classPair = ClassNamePairs.FirstOrDefault( x => x.Name == className ) ;
      if ( classPair == null ) {
        return null ;
      }

      return Activator.CreateInstance( classPair.ClassType ) as SuperType ;
    }

    private static Lazy<ClassNamePair[]> LazyClassNamePairs { get ; } = new Lazy<ClassNamePair[]>( () =>
    {
      //var a = Assembly.GetExecutingAssembly();

      //var b = a.GetTypes();

      //var c = b.Where(x => x.IsClass && x.IsSealed);
      //    .Select(x => new { Type = x, Attributes = x.GetCustomAttributes<AttributeType>() })
      //    .Where(x => x.Attributes.Any())
      //    .SelectMany(x => x.Attributes.Select(y => new ClassNamePair { Name = y?.Name, ClassType = x.Type }))
      //    .Where(x => !string.IsNullOrEmpty(x.Name))
      //    .ToArray();
      return Assembly.GetExecutingAssembly().GetTypes().Where( x => x.IsClass && x.IsSealed )
        .Select( x => new { Type = x, Attributes = x.GetCustomAttributes<AttributeType>() } )
        .Where( x => x.Attributes.Any() )
        .SelectMany( x => x.Attributes.Select( y => new ClassNamePair { Name = y?.Name, ClassType = x.Type } ) )
        .Where( x => ! string.IsNullOrEmpty( x.Name ) ).ToArray() ;
    } ) ;

    private static ClassNamePair[] ClassNamePairs { get ; } = LazyClassNamePairs.Value ;

    private class ClassNamePair
    {
      public string Name { get ; set ; }
      public Type ClassType { get ; set ; }

      public override string ToString() => $"{Name} : {ClassType}" ;
    }
  }

  public abstract class NameAttribute : Attribute
  {
    public string Name { get ; }

    public NameAttribute( string name )
    {
      Name = name ;
    }

    public override object TypeId => this ;
  }
}