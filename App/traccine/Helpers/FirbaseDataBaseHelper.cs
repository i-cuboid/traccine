﻿using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using traccine.Models;

namespace traccine.Helpers
{
    public class FirbaseDataBaseHelper
    {
        public FirebaseClient firebase { get; set; }

        private static readonly object Instancelock = new object();
        public static FirbaseDataBaseHelper _instance = null;
        public static FirbaseDataBaseHelper GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Instancelock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FirbaseDataBaseHelper();
                        }
                    }

                }
                return _instance;
            }
        }
        public FirbaseDataBaseHelper()
        {
            firebase = new FirebaseClient("https://traccine.firebaseio.com/");
        }
        public async Task<List<UserProfile>> GetAllPersons()
        {
            try
            {
                return (await firebase
            .Child("UserProfile")
            .OnceAsync<UserProfile>()).Select(item => new UserProfile
            {
                Name = item.Object.Name,
                Email = item.Object.Email,
                Picture = item.Object.Picture,
                Id = item.Object.Id,
                IsInfected = item.Object.IsInfected,
                PhoneNumber = item.Object.PhoneNumber,
                FcmToken = item.Object.FcmToken,
                IsTermsAndConditionsAccepted = item.Object.IsTermsAndConditionsAccepted
            }).ToList();
            }
            catch (Exception ex)
            {
                return new List<UserProfile>();
            }

        }

        public async Task AddPerson(string personId, string name, string email, string phonenumber, Uri picture)
        {
            try
            {
                await firebase
              .Child("UserProfile")
              .PostAsync(new UserProfile() { Id = personId, Name = name, Email = email, PhoneNumber = phonenumber, Picture = picture });
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task<UserProfile> GetPerson(string email)
        {
            try
            {
                var allPersons = await GetAllPersons();
                await firebase
                  .Child("UserProfile")
                  .OnceAsync<UserProfile>();
                return allPersons.Where(a => a.Email == email).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new UserProfile();
            }
        }
        public async Task<UserProfile> GetPersonByID(string id)
        {
            try
            {
                var allPersons = await GetAllPersons();
                await firebase
                  .Child("UserProfile")
                  .OnceAsync<UserProfile>();
                return allPersons.Where(a => a.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return new UserProfile();
            }
        }

        public async Task UpdatePerson(string id, string phonenumber)
        {
            try
            {
                var toUpdatePerson = (await firebase
              .Child("UserProfile")
              .OnceAsync<UserProfile>()).Where(a => a.Object.Id == id).FirstOrDefault();

                await firebase
                  .Child("UserProfile")
                  .Child(toUpdatePerson.Key)
                  .PutAsync(new UserProfile()
                  {
                      Email = toUpdatePerson.Object.Email,
                      Id = toUpdatePerson.Object.Id,
                      Name = toUpdatePerson.Object.Name,
                      Picture = toUpdatePerson.Object.Picture,
                      IsInfected = toUpdatePerson.Object.IsInfected,
                      FcmToken = toUpdatePerson.Object.FcmToken,
                      IsTermsAndConditionsAccepted = toUpdatePerson.Object.IsTermsAndConditionsAccepted,
                      PhoneNumber = phonenumber
                  });
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public async Task Updateisinfected(string id, Boolean isinfected)
        {
            try
            {
                var toUpdatePerson = (await firebase
              .Child("UserProfile")
              .OnceAsync<UserProfile>()).Where(a => a.Object.Id == id).FirstOrDefault();

                await firebase
                  .Child("UserProfile")
                  .Child(toUpdatePerson.Key)
                  .PutAsync(new UserProfile()
                  {
                      Email = toUpdatePerson.Object.Email,
                      Id = toUpdatePerson.Object.Id,
                      Name = toUpdatePerson.Object.Name,
                      Picture = toUpdatePerson.Object.Picture,
                      IsInfected = isinfected,
                      FcmToken = toUpdatePerson.Object.FcmToken,
                      IsTermsAndConditionsAccepted = toUpdatePerson.Object.IsTermsAndConditionsAccepted,
                      PhoneNumber = toUpdatePerson.Object.PhoneNumber
                  });
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public async Task UpdateFcmToken(string id, string FcmToken)
        {
            try
            {
                var toUpdatePerson = (await firebase
              .Child("UserProfile")
              .OnceAsync<UserProfile>()).Where(a => a.Object.Id == id).FirstOrDefault();

                await firebase
                  .Child("UserProfile")
                  .Child(toUpdatePerson.Key)
                  .PutAsync(new UserProfile()
                  {
                      Email = toUpdatePerson.Object.Email,
                      Id = toUpdatePerson.Object.Id,
                      Name = toUpdatePerson.Object.Name,
                      Picture = toUpdatePerson.Object.Picture,
                      PhoneNumber = toUpdatePerson.Object.PhoneNumber,
                      IsTermsAndConditionsAccepted = toUpdatePerson.Object.IsTermsAndConditionsAccepted,
                      IsInfected = toUpdatePerson.Object.IsInfected,
                      FcmToken = FcmToken
                  });
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public async Task UpdateTermsandcondtions(string id, Boolean isAccepted)
        {
            try
            {
                var toUpdatePerson = (await firebase
              .Child("UserProfile")
              .OnceAsync<UserProfile>()).Where(a => a.Object.Id == id).FirstOrDefault();

                await firebase
                  .Child("UserProfile")
                  .Child(toUpdatePerson.Key)
                  .PutAsync(new UserProfile()
                  {
                      Email = toUpdatePerson.Object.Email,
                      Id = toUpdatePerson.Object.Id,
                      Name = toUpdatePerson.Object.Name,
                      Picture = toUpdatePerson.Object.Picture,
                      PhoneNumber = toUpdatePerson.Object.PhoneNumber,
                      IsTermsAndConditionsAccepted = isAccepted,
                      IsInfected = toUpdatePerson.Object.IsInfected,
                      FcmToken = toUpdatePerson.Object.FcmToken
                  });
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public async Task DeletePerson(string personId)
        {
            try
            {
                var toDeletePerson = (await firebase
              .Child("UserProfile")
              .OnceAsync<UserProfile>()).Where(a => a.Object.Id == personId).FirstOrDefault();
                await firebase.Child("Persons").Child(toDeletePerson.Key).DeleteAsync();
            }
            catch (Exception ex)
            {
                return;
            }

        }
    }
}
