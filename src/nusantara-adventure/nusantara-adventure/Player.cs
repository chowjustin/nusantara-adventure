using System;
using System.Collections.Generic;
namespace nusantara_adventure
{
    internal class Player : Character
    {
        public int Score { get; set; }
        public Costume CurrentCostume { get; set; }
        public List<Costume> OwnedCostumes { get; set; }

        public Player(string name, int x, int y, int health, int speed)
            : base(name, x, y, health, speed)
        {
            OwnedCostumes = new List<Costume>();
        }

        public void CollectItem(Item item)
        {
            Score += item.Value;
            item.ApplyEffect(this);
        }

        public void AddCostume(Costume costume)
        {
            OwnedCostumes.Add(costume);
        }

        public void ChangeCostume(Costume costume)
        {
            if (OwnedCostumes.Contains(costume))
            {
                CurrentCostume = costume;
            }
        }

        public void RemoveCostume(Costume costume)
        {
            OwnedCostumes.Remove(costume);
            if (CurrentCostume == costume)
            {
                CurrentCostume = null;
            }
        }

        public void Move(int horizontalInput, int verticalInput)
        {
            // Horizontal movement
            X += horizontalInput * Speed;
            // Jump logic
            if (verticalInput < 0 && IsGrounded)
            {
                Jump();
            }
        }

        public void Attack(Enemy enemy)
        {
            // Logic for player attacking an enemy
        }
    }
}