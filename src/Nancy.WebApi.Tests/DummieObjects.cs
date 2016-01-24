using System.Collections.Generic;

namespace Nancy.WebApi.Tests
{
    /// <summary>
    /// User Detail.
    /// </summary>
    public class User
    {
        /// <summary>
        /// user Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User Name.
        /// </summary>
        public Name Name { get; set; }   

        /// <summary>
        /// User Gender.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// User phones .
        /// </summary>
        public List<Phone> Phones { get; set; }    

        /// <summary>
        /// User addresses.
        /// </summary>
        public List<Address> Addresses { get; set; }    
    }

    /// <summary>
    /// Address Detail.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Address Type.
        /// </summary>
        public AddressType AddressType { get; set; }
        /// <summary>
        /// Street.
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// This is City property.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Yes, it is state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// and zip code.
        /// </summary>
        public string Zip { get; set; }
    }

    /// <summary>
    /// Address Type, primary or vacation.
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Primary home address
        /// </summary>
        Primary,

        /// <summary>
        /// home for vacation.
        /// </summary>
        Vacation
    }

    /// <summary>
    /// Phones, cell, home, or work phone.
    /// </summary>
    public class Phone
    {
        /// <summary>
        /// phone type
        /// </summary>
        public PhoneType PhoneType { get; set; }

        /// <summary>
        /// phone number.
        /// </summary>
        public string Number { get; set; }
    }

    public enum PhoneType
    {
        /// <summary>
        /// cell phone
        /// </summary>
        Cell,

        /// <summary>
        /// home phone
        /// </summary>
        Home,

        /// <summary>
        /// work phone.
        /// </summary>
        Work
    }

    /// <summary>
    /// Gender male or female
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// male
        /// </summary>
        Male,

        /// <summary>
        /// female
        /// </summary>
        Female
    }

    /// <summary>
    /// User Name.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// user first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// user middle name.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// user last name.
        /// </summary>
        public string LastName { get; set; }
    }
  
}
