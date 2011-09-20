// Copyright 2010, 2011 Bastien Hofmann <kamo@cfagn.net>
//
// This file is part of Blib.
//
// Blib is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Blib is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with Blib.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Blib
{
    public abstract class ValidatorAttribute : Attribute
    {
        public abstract void Validate(Element element, PropertyInfo property, object value);
    }

    public class RequiredAttribute : ValidatorAttribute
    {
        public RequiredAttribute()
        {
            Required = true;
        }

        public bool Required { get; set; }

        public override void Validate(Element element, PropertyInfo property, object value)
        {
            if (value == null)
            {
                throw new ValidationException(string.Format("Missing argument {0} for {1}!", property.Name, element.Name));
            }
        }
    }

    public class StringValidatorAttribute : ValidatorAttribute
    {
        public StringValidatorAttribute()
        {
            AllowEmpty = true;
        }

        public bool AllowEmpty { get; set; }

        public override void Validate(Element element, PropertyInfo property, object value)
        {
            if (property.PropertyType != typeof(string))
            {
                throw new ValidationWrongTypeException(string.Format("The {0} can only be used on {1} properties", GetType().Name, "string"));
            }

            if (!AllowEmpty)
            {
                if (string.IsNullOrEmpty((string)value))
                {
                    throw new ValidationException(string.Format("Argument {0} for {1} can't be empty!", property.Name, element.Name));
                }
            }
        }
    }

    [Serializable]
    public class ValidationException : BuildException
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ValidationWrongTypeException : BuildException
    {
        public ValidationWrongTypeException() { }
        public ValidationWrongTypeException(string message) : base(message) { }
        public ValidationWrongTypeException(string message, Exception inner) : base(message, inner) { }
        protected ValidationWrongTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
