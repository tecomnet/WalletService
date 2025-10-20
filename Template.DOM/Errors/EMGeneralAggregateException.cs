using System.Runtime.Serialization;

namespace Template.DOM.Errors;

public class EMGeneralAggregateException : AggregateException
{
    public EMGeneralAggregateException(EMGeneralException exception)
        : base((Exception) exception)
    {
    }

    public EMGeneralAggregateException(List<EMGeneralException> exceptions)
        : base((IEnumerable<Exception>) exceptions)
    {
    }

    protected EMGeneralAggregateException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public EMGeneralException? InnerException => (EMGeneralException) base.InnerException;

    public List<EMGeneralException>? InnerExceptions
    {
        get
        {
            return base.InnerExceptions.Select<Exception, EMGeneralException>((Func<Exception, EMGeneralException>) (innerException => (EMGeneralException) innerException)).ToList<EMGeneralException>();
        }
    }
}