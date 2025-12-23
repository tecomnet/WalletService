using System.Runtime.Serialization;

namespace Wallet.DOM.Errors;

/// <summary>
/// Representa uno o más errores generales que ocurren durante la ejecución de una aplicación.
/// Esta excepción agrega una o más instancias de <see cref="EMGeneralException"/>.
/// </summary>
public class EMGeneralAggregateException : AggregateException
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EMGeneralAggregateException"/> con una única excepción interna.
    /// </summary>
    /// <param name="exception">La excepción general única que causó esta excepción agregada.</param>
    public EMGeneralAggregateException(EMGeneralException exception)
        : base(innerExceptions: exception)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EMGeneralAggregateException"/> con una colección de excepciones internas.
    /// </summary>
    /// <param name="exceptions">Una colección de excepciones generales que causaron esta excepción agregada.</param>
    public EMGeneralAggregateException(List<EMGeneralException> exceptions)
        : base(innerExceptions: exceptions)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EMGeneralAggregateException"/> con datos serializados.
    /// </summary>
    /// <param name="info">El objeto que contiene los datos serializados del objeto.</param>
    /// <param name="context">La información contextual sobre la fuente o el destino de la serialización.</param>
    protected EMGeneralAggregateException(SerializationInfo info, StreamingContext context)
        : base(info: info, context: context)
    {
    }

    /// <summary>
    /// Obtiene la excepción general interna que causó esta excepción agregada.
    /// Es nulo si la excepción agregada se creó con múltiples excepciones.
    /// </summary>
    public EMGeneralException? InnerException => (EMGeneralException) base.InnerException;

    /// <summary>
    /// Obtiene una lista de las excepciones generales internas que causaron esta excepción agregada.
    /// </summary>
    public List<EMGeneralException>? InnerExceptions
    {
        get
        {
            // Convierte la colección base de excepciones a una lista de EMGeneralException.
            return base.InnerExceptions.Select(selector: (Func<Exception, EMGeneralException>) (innerException => (EMGeneralException) innerException)).ToList();
        }
    }
}