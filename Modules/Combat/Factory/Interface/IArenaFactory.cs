using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Factory.Interface
{
    public interface IArenaFactory
    {
        public Arena Create(ArenaCreation arenaCreation);
    }
}