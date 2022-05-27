using System.Runtime.Serialization;

namespace FarmManager.Domain.Exceptions
{
    [Serializable]
    public class EntityDependencyException : Exception
    {
        public EntityDependencyException()
        {
        }

        public EntityDependencyException(string? message) : base(message)
        {
        }

        public EntityDependencyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EntityDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
