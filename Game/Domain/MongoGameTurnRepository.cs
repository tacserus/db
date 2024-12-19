using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameTurnRepository : IGameTurnRepository
    {
        private const string CollectionName = "gameTurns";
        private readonly IMongoCollection<GameTurnEntity> gameTurnEntityCollection;

        public MongoGameTurnRepository(IMongoDatabase database)
        {
            gameTurnEntityCollection = database.GetCollection<GameTurnEntity>(CollectionName);
            
            var indexModel = new CreateIndexModel<GameTurnEntity>(
                Builders<GameTurnEntity>.IndexKeys.Descending(x => x.TurnIndex));
            
            gameTurnEntityCollection.Indexes.CreateOne(indexModel);
        }
        
        public IEnumerable<GameTurnEntity> GetLastTurns(Guid gameId, int limit)
        {
            return gameTurnEntityCollection.Find(x => x.GameId == gameId)
                .SortByDescending(x => x.TurnIndex)
                .Limit(limit)
                .ToList();
        }
        
        public void Insert(GameTurnEntity gameTurnEntity)
        {
            gameTurnEntityCollection.InsertOne(gameTurnEntity);
        }

    }
}