﻿using DogGo.Models;
using Microsoft.Data.SqlClient;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walk> GetAllWalks()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, w.Date, w.Duration, w.WalkerId, w.DogId, o.Name AS OwnerName, d.Name AS DogName
                        FROM Walks w
                        INNER JOIN Dog d ON w.DogId = d.Id
                        INNER JOIN Owner o ON d.OwnerId = o.Id";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog
                            {
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                                Owner = new Owner
                                {
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                }
                            }
                        };

                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }

        public Walk GetWalkById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, w.Date, w.Duration, w.WalkerId, w.DogId, o.Name AS OwnerName, d.Name AS DogName
                                FROM Walks w
                                INNER JOIN Dog d ON w.DogId = d.Id
                                INNER JOIN Owner o ON d.OwnerId = o.Id
                                WHERE w.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog
                            {
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                                Owner = new Owner
                                {
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                }
                            }
                        };

                        reader.Close();
                        return walk;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void AddWalk(Walk walk, List<int> SelectedDogIds)
        {
            foreach (int dogId in SelectedDogIds)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {

                        cmd.CommandText = @"
                        INSERT INTO Walk (Date, Duration, WalkerId, DogId, WalkStatusId)
                        OUTPUT INSERTED.ID
                        VALUES (@date, @duration, @walkerId, @dogId, @statusId);
                    ";

                        cmd.Parameters.AddWithValue("@date", walk.Date);
                        cmd.Parameters.AddWithValue("@duration", walk.Duration);
                        cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
                        cmd.Parameters.AddWithValue("@dogId", dogId);
                        cmd.Parameters.AddWithValue("@statusId", walk.StatusId);

                        int id = (int)cmd.ExecuteScalar();

                        walk.Id = id;
                    }
                }
            }
        }

        public void UpdateWalk(Walk walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Walk SET Date = @date, Duration = @duration, WalkerId = @walkerId, DogId = @dogId WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@date", walk.Date);
                    cmd.Parameters.AddWithValue("@duration", walk.Duration);
                    cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
                    cmd.Parameters.AddWithValue("@dogId", walk.DogId);
                    cmd.Parameters.AddWithValue("@id", walk.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteWalk(List<int> SelectedWalkIds)
        {
            foreach (int walkId in SelectedWalkIds)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                                DELETE FROM Walk
                                WHERE Id = @walkId
                            ";

                        cmd.Parameters.AddWithValue("@walkId", walkId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, w.Date, w.Duration, w.WalkStatusId, w.DogId, o.Name AS OwnerName, d.Name AS DogName
                                FROM Walks w
                                INNER JOIN Dog d ON w.DogId = d.Id
                                INNER JOIN Owner o ON d.OwnerId = o.Id
                                WHERE w.WalkerId = @walkerId";
                    cmd.Parameters.AddWithValue("@walkerId", walkerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();
                    while (reader.Read())
                    {
                        Walk walk = new Walk
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            StatusId = reader.GetInt32(reader.GetOrdinal("WalkStatusId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog
                            {
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                                Owner = new Owner
                                {
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                }
                            }
                        };

                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }
    }
}
