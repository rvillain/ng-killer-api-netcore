using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NgKillerApiCore
{
    public class Constantes
    {
        public const string ACTTION_TYPE_KILL="kill";
        public const string ACTTION_TYPE_UNMASK="unmask";
        public const string ACTTION_TYPE_WRONG_KILLER="wrong_killer";
        public const string ACTTION_TYPE_ERROR_DEATH="error_death";
        public const string ACTTION_TYPE_SUICIDE="suicide";
        public const string ACTTION_TYPE_GAME_STARTED="game_started";
                      
        public const string REQUEST_TYPE_JOIN_ROOM="join-room";
                      
        public const string REQUEST_TYPE_ASK_KILL="ask-kill";
        public const string REQUEST_TYPE_CONFIRM_KILL="confirm-kill";
        public const string REQUEST_TYPE_UNCONFIRM_KILL="unconfirm-kill";
                       
        public const string REQUEST_TYPE_ASK_UNMASK="ask-unmask";
        public const string REQUEST_TYPE_CONFIRM_UNMASK="confirm-unmask";
        public const string REQUEST_TYPE_UNCONFIRM_UNMASK="unconfirm-unmask";
                       
        public const string REQUEST_TYPE_AGENT_UPDATE="agent-update";
        public const string REQUEST_TYPE_CHANGE_MISSION="change-mission";
        public const string REQUEST_TYPE_SUICIDE="suicide";
        public const string REQUEST_TYPE_NEW_ACTION="new-action";
        public const string REQUEST_TYPE_NEW_AGENT="new-agent";
        public const string REQUEST_TYPE_GAME_STATUS="game-status";
        public const string REQUEST_TYPE_ACTION_ERROR="action-error";
        public const string REQUEST_TYPE_TRIBUNAL_STATUS="tribunal-status";
    }
}
