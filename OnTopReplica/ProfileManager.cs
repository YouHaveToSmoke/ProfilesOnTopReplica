using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OnTopReplica {
    
    /// <summary>
    /// Manages saving and loading of profiles.
    /// </summary>
    public class ProfileManager {
        
        private static readonly string ProfilesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "OnTopReplica",
            "Profiles"
        );
        
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(Profile));
        
        static ProfileManager() {
            if (!Directory.Exists(ProfilesDirectory)) {
                Directory.CreateDirectory(ProfilesDirectory);
            }
        }
        
        /// <summary>
        /// Saves a profile to disk.
        /// </summary>
        public static void SaveProfile(Profile profile) {
            if (string.IsNullOrWhiteSpace(profile.Name)) {
                throw new ArgumentException("Profile name cannot be empty.");
            }
            
            profile.LastModified = DateTime.Now;
            
            string fileName = GetSafeFileName(profile.Name) + ".xml";
            string filePath = Path.Combine(ProfilesDirectory, fileName);
            
            using (var writer = new StreamWriter(filePath)) {
                Serializer.Serialize(writer, profile);
            }
            
            Log.Write("Profile saved: " + profile.Name);
        }
        
        /// <summary>
        /// Loads a profile from disk by name.
        /// </summary>
        public static Profile LoadProfile(string profileName) {
            string fileName = GetSafeFileName(profileName) + ".xml";
            string filePath = Path.Combine(ProfilesDirectory, fileName);
            
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException("Profile not found: " + profileName);
            }
            
            using (var reader = new StreamReader(filePath)) {
                var profile = (Profile)Serializer.Deserialize(reader);
                Log.Write("Profile loaded: " + profile.Name);
                return profile;
            }
        }
        
        /// <summary>
        /// Gets all available profile names.
        /// </summary>
        public static List<string> GetProfileNames() {
            if (!Directory.Exists(ProfilesDirectory)) {
                return new List<string>();
            }
            
            return Directory.GetFiles(ProfilesDirectory, "*.xml")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .ToList();
        }
        
        /// <summary>
        /// Gets all available profiles.
        /// </summary>
        public static List<Profile> GetAllProfiles() {
            var profiles = new List<Profile>();
            var profileNames = GetProfileNames();
            
            foreach (var name in profileNames) {
                try {
                    profiles.Add(LoadProfile(name));
                }
                catch (Exception ex) {
                    Log.Write("Error loading profile " + name + ": " + ex.Message);
                }
            }
            
            return profiles.OrderBy(p => p.Name).ToList();
        }
        
        /// <summary>
        /// Deletes a profile from disk.
        /// </summary>
        public static void DeleteProfile(string profileName) {
            string fileName = GetSafeFileName(profileName) + ".xml";
            string filePath = Path.Combine(ProfilesDirectory, fileName);
            
            if (File.Exists(filePath)) {
                File.Delete(filePath);
                Log.Write("Profile deleted: " + profileName);
            }
        }
        
        /// <summary>
        /// Checks if a profile exists.
        /// </summary>
        public static bool ProfileExists(string profileName) {
            string fileName = GetSafeFileName(profileName) + ".xml";
            string filePath = Path.Combine(ProfilesDirectory, fileName);
            return File.Exists(filePath);
        }
        
        /// <summary>
        /// Converts a profile name to a safe file name.
        /// </summary>
        private static string GetSafeFileName(string name) {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }
    }
}
