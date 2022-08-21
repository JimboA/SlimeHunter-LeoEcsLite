using System;

namespace Client.AppData
{
       [AttributeUsage (AttributeTargets.Struct)]
       public sealed class GenerateDataProviderAttribute : Attribute
       {
          private string _path;
    
          public GenerateDataProviderAttribute()
          {
          }
    
          public GenerateDataProviderAttribute(string path)
          {
             _path = path;
          }
       }
}
