//-----------------------------------------------------------------------
// <copyright file="LinqUtilities.cs">
// Copyright © Ladislau Molnar. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Syinpo.Core.Linq
{
    public static class MoreLinq {
        public static IEnumerable<IEnumerable<T>> GetBatches<T>(this IEnumerable<T> source, int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException("batchSize");
            }

            T[] batch = new T[batchSize];
            int indexInBatch = 0;

            foreach (T sourceElement in source)
            {
                batch[indexInBatch++] = sourceElement;

                if (indexInBatch == batchSize)
                {
                    yield return batch;
                    indexInBatch = 0;
                }
            }

            if (indexInBatch > 0)
            {
                yield return batch.Take(indexInBatch);
            }
        }
    }
}
