using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.Angular.Classes
{
    public static class Mapper
    {
        public static T ConvertViewModel<T, U>(U model) where T : new()
        {
            T output = new();

            var colsModel = model.GetType().GetProperties();
            var colsOutput = output.GetType().GetProperties();

            foreach (var colModel in colsModel)
            {
                foreach (var colOutput in colsOutput)
                {
                    if (colModel.PropertyType == colOutput.PropertyType && colModel.Name == colOutput.Name)
                    {
                        var tempType = colModel.PropertyType;

                        if (tempType.IsGenericType && tempType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            tempType = Nullable.GetUnderlyingType(tempType);
                        }
                        colOutput.SetValue(output, colModel.GetValue(model) != null ? Convert.ChangeType(colModel.GetValue(model), tempType) : null);
                        break;
                    }
                }
            }

            return output;
        }

        public static List<T> ConvertListViewModel<T, U>(List<U> model) where T : new()
        {
            List<T> output = new();
            T entry = new();

            if (model == null || model.Count < 1)
            {
                return new List<T>();
            }

            var colsModel = model[0].GetType().GetProperties();
            var colsOutput = entry.GetType().GetProperties();

            for (int i = 0; i < model.Count; i++)
            {
                entry = new T();

                foreach (var colModel in colsModel)
                {
                    foreach (var colOutput in colsOutput)
                    {
                        if (colModel.PropertyType == colOutput.PropertyType && colModel.Name == colOutput.Name)
                        {
                            var tempType = colModel.PropertyType;

                            if (tempType.IsGenericType && tempType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                tempType = Nullable.GetUnderlyingType(tempType);
                            }

                            colOutput.SetValue(entry, colModel.GetValue(model[i]) != null ? Convert.ChangeType(colModel.GetValue(model[i]), colOutput.PropertyType) : null);
                            break;
                        }
                    }
                }

                output.Add(entry);
            }

            return output;
        }
    }
}