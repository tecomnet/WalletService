using System.Runtime.Serialization;

namespace Wallet.DOM.Errors;

public class EMGeneralAggregateException : AggregateException
{
    public EMGeneralAggregateException(EMGeneralException exception)
        : base(innerExceptions: (Exception) exception)
    {
    }

    public EMGeneralAggregateException(List<EMGeneralException> exceptions)
        : base(innerExceptions: (IEnumerable<Exception>) exceptions)
    {
    }

    protected EMGeneralAggregateException(SerializationInfo info, StreamingContext context)
        : base(info: info, context: context)
    {
    }

    public EMGeneralException? InnerException => (EMGeneralException) base.InnerException;

    public List<EMGeneralException>? InnerExceptions
    {
        get
        {
            return base.InnerExceptions.Select<Exception, EMGeneralException>(selector: (Func<Exception, EMGeneralException>) (innerException => (EMGeneralException) innerException)).ToList<EMGeneralException>();
        }
    }
}