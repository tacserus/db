using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameEntityCollection;
        
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            gameEntityCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameEntityCollection.InsertOne(game);
            
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            return gameEntityCollection.Find(x => x.Id == gameId).SingleOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameEntityCollection.UpdateOne(
                x => x.Id == game.Id, 
                new BsonDocumentUpdateDefinition<GameEntity>(new BsonDocument("$set", game.ToBsonDocument()))
                );
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            return gameEntityCollection.Find(x => x.Status == GameStatus.WaitingToStart).Limit(limit).ToList();
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            var updateResult = gameEntityCollection.UpdateOne(
                x => x.Status == GameStatus.WaitingToStart && x.Id == game.Id, 
                new BsonDocumentUpdateDefinition<GameEntity>(new BsonDocument("$set", game.ToBsonDocument()))
                );
            
            return updateResult.ModifiedCount > 0;
        }
    }
}