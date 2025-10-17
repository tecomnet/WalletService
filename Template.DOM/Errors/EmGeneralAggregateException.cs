using System.Runtime.Serialization;

namespace Template.DOM.Errors;

public class EmGeneralAggregateException : AggregateException
{
    public EmGeneralAggregateException(EmGeneralException exception)
        : base((Exception) exception)
    {
    }

    public EmGeneralAggregateException(List<EmGeneralException> exceptions)
        : base((IEnumerable<Exception>) exceptions)
    {
    }

    protected EmGeneralAggregateException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public EmGeneralException? InnerException => (EmGeneralException) base.InnerException;

    public List<EmGeneralException>? InnerExceptions
    {
        get
        {
            return base.InnerExceptions.Select<Exception, EmGeneralException>((Func<Exception, EmGeneralException>) (innerException => (EmGeneralException) innerException)).ToList<EmGeneralException>();
        }
    }
}