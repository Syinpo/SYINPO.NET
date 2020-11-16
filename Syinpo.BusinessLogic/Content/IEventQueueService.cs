using System.Collections.Generic;
using System.Threading.Tasks;
using Syinpo.Core;
using Syinpo.Core.Domain.Poco;

namespace Syinpo.BusinessLogic.Content
{
    public interface IEventQueueService
    {
        Task InsertEventQueue( EventQueue eventQueue );
        Task InsertRangeEventQueue( List<EventQueue> eventQueues );
        Task UpdateEventQueue( EventQueue eventQueue );
        Task UpdateRangeEventQueue(List<EventQueue> eventQueues);
        Task DeleteEventQueue( EventQueue eventQueue );
        Task DeleteRangeEventQueue( List<EventQueue> eventQueues );
        Task<EventQueue> GetEventQueueById( int eventQueueId );

        Task<PageList<EventQueue>> SearchEventQueues( string version = null, string routeName = null, bool onlyNoOut = true,
            int maxRetry = 3, int pageIndex = 0, int pageSize = 100000 );
    }
}