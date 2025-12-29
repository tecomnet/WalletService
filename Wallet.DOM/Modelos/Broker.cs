using System.ComponentModel.DataAnnotations;
using Wallet.DOM.Comun;
using Wallet.DOM.Errors;

namespace Wallet.DOM.Modelos
{
    /// <summary>
    /// Representa un intermediario o broker en el sistema.
    /// </summary>
    public class Broker : ValidatablePersistentObjectLogicalDelete
    {
        /// <summary>
        /// Define las restricciones de las propiedades para la validación del objeto <see cref="Broker"/>.
        /// </summary>
        protected override List<PropertyConstraint> PropertyConstraints =>
        [
            PropertyConstraint.StringPropertyConstraint(
                propertyName: nameof(Nombre),
                isRequired: true,
                maximumLength: 100,
                minimumLength: 1)
        ];

        /// <summary>
        /// Obtiene el nombre del broker.
        /// </summary>
        [Required]
        [MaxLength(length: 100)]
        public string Nombre { get; private set; }

        /// <summary>
        /// Constructor privado para uso exclusivo de Entity Framework.
        /// </summary>
        protected Broker() : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Broker"/>.
        /// </summary>
        /// <param name="nombre">El nombre del broker.</param>
        /// <param name="creationUser">El identificador del usuario que crea el registro.</param>
        public Broker(string nombre, Guid creationUser) : base(creationUser: creationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            Nombre = nombre;
        }

        /// <summary>
        /// Actualiza los datos del broker.
        /// </summary>
        /// <param name="nombre">El nuevo nombre del broker.</param>
        /// <param name="modificationUser">El identificador del usuario que modifica el registro.</param>
        public void Update(string nombre, Guid modificationUser)
        {
            var exceptions = new List<EMGeneralException>();
            IsPropertyValid(propertyName: nameof(Nombre), value: nombre, exceptions: ref exceptions);
            if (exceptions.Count > 0)
            {
                throw new EMGeneralAggregateException(exceptions: exceptions);
            }

            if (Nombre == nombre) return;

            Nombre = nombre;
            base.Update(modificationUser: modificationUser);
        }
    }
}
