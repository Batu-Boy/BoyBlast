public interface IDamageable
{
    public int Health { get; set; }

    public void TakeDamage(Cell from, int amount = 1);
}