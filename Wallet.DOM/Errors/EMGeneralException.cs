namespace Wallet.DOM.Errors;

/// <summary>
/// Representa una excepción general utilizada en el dominio de Wallet.DOM.
/// Esta excepción está diseñada para contener información detallada sobre el error del servicio.
/// </summary>
public class EMGeneralException : Exception
{
    /// <summary>
    /// Obtiene la interfaz del error del servicio que originó esta excepción.
    /// </summary>
    public IServiceError ServiceError { get; }
    /// <summary>
    /// Obtiene el código identificador del error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Obtiene el título descriptivo de la excepción.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Obtiene la descripción detallada de la excepción.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Obtiene una lista opcional de contenidos dinámicos que se pueden usar en la descripción.
    /// </summary>
    public List<string>? DescriptionDynamicContents { get; }
    /// <summary>
    /// Obtiene el nombre del servicio donde ocurrió la excepción.
    /// </summary>
    public string ServiceName { get; }
    /// <summary>
    /// Obtiene la instancia específica del servicio donde ocurrió la excepción.
    /// </summary>
    public string ServiceInstance { get; }

    /// <summary>
    /// Obtiene la ubicación del servicio donde se produjo la excepción.
    /// </summary>
    public string ServiceLocation { get; }
    /// <summary>
    /// Obtiene el módulo dentro del servicio donde se originó la excepción.
    /// </summary>
    public string Module { get; }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EMGeneralException"/> con un mensaje, código, título, descripción y detalles del servicio.
    /// </summary>
    /// <param name="message">El mensaje que describe el error.</param>
    /// <param name="code">El código del error.</param>
    /// <param name="title">El título de la excepción.</param>
    /// <param name="description">La descripción detallada de la excepción.</param>
    /// <param name="serviceName">El nombre del servicio donde ocurrió la excepción.</param>
    /// <param name="serviceInstance">La instancia específica del servicio (opcional).</param>
    /// <param name="serviceLocation">La ubicación del servicio (opcional).</param>
    /// <param name="module">El módulo dentro del servicio.</param>
    /// <param name="descriptionDynamicContents">Contenido dinámico opcional para la descripción.</param>
    public EMGeneralException(
        string message,
        string code,
        string title,
        string description,
        string serviceName,
        string? serviceInstance,
        string? serviceLocation,
        string module,
        List<object>? descriptionDynamicContents = null)
        : base(message: message)
    {
        this.Code = code;
        this.Title = title;
        this.Description = description;
        // Procesa el contenido dinámico para la descripción
        this.DescriptionDynamicContents = EMGeneralException.ProcessDynamicContent(dynamicContent: descriptionDynamicContents);
        this.ServiceName = serviceName;
        // Asigna "NA" si la instancia del servicio es nula
        this.ServiceInstance = serviceInstance ?? "NA";
        // Asigna "NA" si la ubicación del servicio es nula
        this.ServiceLocation = serviceLocation ?? "NA";
        this.Module = module;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EMGeneralException"/> utilizando un objeto <see cref="IServiceError"/>.
    /// </summary>
    /// <param name="serviceError">La instancia de <see cref="IServiceError"/> que contiene los detalles del error.</param>
    /// <param name="serviceName">El nombre del servicio donde ocurrió la excepción.</param>
    /// <param name="module">El módulo dentro del servicio. Por defecto es "DOM".</param>
    /// <param name="descriptionDynamicContents">Contenido dinámico opcional para la descripción.</param>
    public EMGeneralException(
        IServiceError serviceError,
        string serviceName,
        string module = "DOM",
        List<object>? descriptionDynamicContents = null)
        : base(message: serviceError.Message)
    {
        this.ServiceError = serviceError; // Asigna la instancia de IServiceError
        this.Code = serviceError.ErrorCode;
        this.Title = serviceError.Message;
        // Genera la descripción usando el método Description del IServiceError
        this.Description = serviceError.Description(args: descriptionDynamicContents?.ToArray());
        // Procesa el contenido dinámico para la descripción
        this.DescriptionDynamicContents = EMGeneralException.ProcessDynamicContent(dynamicContent: descriptionDynamicContents);
        this.ServiceName = serviceName;
        this.Module = module;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EMGeneralException"/> con un mensaje de error especificado y una referencia a la excepción interna que es la causa de esta excepción.
    /// </summary>
    /// <param name="message">El mensaje que describe el error.</param>
    /// <param name="inner">La excepción que es la causa de la excepción actual, o una referencia nula (<see langword="null"/>) si no se especifica ninguna excepción interna.</param>
    public EMGeneralException(string message, Exception inner)
        : base(message: message, innerException: inner)
    {
        // Los campos específicos de EMGeneralException no se inicializan en este constructor
        // ya que está diseñado para envolver una excepción existente.
        this.Code = "NA"; // Código por defecto
        this.Title = "Error General"; // Título por defecto
        this.Description = message; // La descripción es el mensaje
        this.ServiceName = "NA"; // Servicio por defecto
        this.ServiceInstance = "NA"; // Instancia por defecto
        this.ServiceLocation = "NA"; // Ubicación por defecto
        this.Module = "NA"; // Módulo por defecto
    }
    
    /// <summary>
    /// Procesa una lista de objetos dinámicos y los convierte a una lista de cadenas.
    /// </summary>
    /// <param name="dynamicContent">La lista de objetos cuyo contenido se va a procesar.</param>
    /// <returns>Una lista de cadenas que representan el contenido dinámico, o <see langword="null"/> si la entrada es nula.</returns>
    private static List<string>? ProcessDynamicContent(List<object>? dynamicContent)
    {
        // Si el contenido dinámico es nulo, devuelve nulo
        if (dynamicContent == null)
            return (List<string>) null;
        
        List<string> stringList = new List<string>();
        // Itera sobre cada objeto en el contenido dinámico y lo convierte a cadena
        foreach (object obj in dynamicContent)
            stringList.Add(item: $"{obj}"); // Usa interpolación de cadenas para convertir el objeto a string
        
        return stringList;
    }
}