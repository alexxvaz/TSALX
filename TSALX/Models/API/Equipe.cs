namespace TSALX.Models.API
{
    public class Equipe : APIBase
    {
        public bool Selecao { get; set; }
        public override string Escudo
        {
            get 
            {
                if ( this.ID > 0 )
                    return $"https://media.api-sports.io/football/teams/{this.ID}.png";
                else
                    return "/Content/img/escudo.png";
            }
        }
    }
}