using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Seguimiento.API.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //Estamos creando un enlazador de modelos para el tipo IEnumerable. Por lo tanto, tenemos que comprobar si nuestro parámetro es del mismo tipo.
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
            //Extraemos el valor (una cadena de GUID separados por comas)
            var providedValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();
            if (string.IsNullOrEmpty(providedValue))//solo verificamos si es nulo o está vacío
            {
                bindingContext.Result = ModelBindingResult.Success(null);//devolvemos nulo como resultado
                return Task.CompletedTask;
            }
            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];//con la ayuda de la reflexión, almacenamos el tipo del que consta el IEnumerable


            //creamos un convertidor a un tipo GUID. Como puede ver, no solo forzamos el tipo de GUID en este enlazador de modelos;
            //en su lugar, inspeccionamos cuál es el tipo anidado del parámetro IEnumerable y luego creamos un convertidor para ese tipo exacto,
            //lo que hace que este enlazador sea genérico.
            var converter = TypeDescriptor.GetConverter(genericType);
            // creamos una matriz de tipo objeto (objectArray) que consta de todos los valores de GUID que enviamos a la API
            var objectArray = providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => converter.ConvertFromString(x.Trim())).ToArray();
            // luego creamos una matriz de tipos de GUID (guidArray), copiamos todos los valores de objectArray a guidArray y asignamos al bindingContext.
            var guidArray = Array.CreateInstance(genericType, objectArray.Length);
            objectArray.CopyTo(guidArray, 0);
            bindingContext.Model = guidArray;
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
