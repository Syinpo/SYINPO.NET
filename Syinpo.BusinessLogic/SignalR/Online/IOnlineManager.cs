using System.Collections.Generic;

namespace Syinpo.BusinessLogic.SignalR.Online
{
    public interface IOnlineManager
    {
        void Add(OnlineClient client);

        bool Remove(string connectionId);

        bool Remove(OnlineClient client);

        OnlineClient GetByConnectionIdOrNull(string connectionId);

        IReadOnlyList<OnlineClient> GetAllClients();


        IReadOnlyList<OnlineClient> GetAllUsers();

        long GetUserOnlinesCount();


        IReadOnlyList<OnlineClient> GetAllByUserId(int userId);

        bool IsOnline(int userId);

        IReadOnlyList<OnlineClient> GetAllByDeviceId(int deviceId);

        bool IsDeviceOnline(int deviceId);

    }
}
