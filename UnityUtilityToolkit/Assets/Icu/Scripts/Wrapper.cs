using System;
using System.Collections.Generic;

namespace Icu
{
    [Serializable]
    public class Wrapper<T>
    {
        public T t;
    }
    
    [Serializable]
    public class SteamResponse
    {
        public FriendsList friendslist;
    }

    [Serializable]
    public class FriendsList
    {
        public List<Friend> friends;
    }

    [Serializable]
    public class Friend
    {
        public string steamid;
        public string relationship;
        public long friend_since;
    }
    
    [System.Serializable]
    public class Player
    {
        public string steamid;
        public int communityvisibilitystate;
        public int profilestate;
        public string personaname;
        public string profileurl;
        public string avatar;
        public string avatarmedium;
        public string avatarfull;
        public string avatarhash;
        public int personastate;
        public string realname;
        public string primaryclanid;
        public int timecreated;
        public int personastateflags;
        public string loccountrycode;
        public string locstatecode;
        public int loccityid;
    }

    [System.Serializable]
    public class Response
    {
        public List<Player> players;
    }

    [System.Serializable]
    public class SteamPlayerData
    {
        public Response response;
    }
}