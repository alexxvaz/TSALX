﻿namespace TSALX.Models.API
{
    public class Liga : APIBase 
    {
        public override string Escudo
        {
            get { return $"https://media.api-sports.io/football/leagues/{this.ID}.png"; }
        }
    }
}