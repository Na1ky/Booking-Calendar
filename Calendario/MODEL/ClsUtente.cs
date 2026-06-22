using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Calendario.MODEL
{
    public class ClsUtente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        
        public string SessionToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
