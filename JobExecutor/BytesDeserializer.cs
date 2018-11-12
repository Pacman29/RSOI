using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace JobExecutor
{
    public class BytesDeserializer<TResult>
    {
        public static TResult Deserialize(BaseJob job)
        {
            TResult res;
            if (job.Bytes != null)
            {
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream(job.Bytes))
                    res = (TResult)bf.Deserialize(ms);
            }
            else
            {
                res = default(TResult);
            }
            return res;
        }
    }
}
