public class ShowDieRoll 
{
  public int Result { get; }
  public int DieMax { get; }

  public ShowDieRoll(int result, int dieMax)
  {
    Result = result;
    DieMax = dieMax;
  }
}
