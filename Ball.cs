using Godot;
using System;

public partial class Ball : CharacterBody2D, IPoolable
{
    [Export] private float radius = 12f;
    private Sprite2D _sprite;
    private CollisionShape2D _collision;
    private float speed = 450f;
    private Paddle attachedPaddle = null;
    private bool isStuck = false;

    private float stuckGap = 20f;

    public void OnSpawn()
    {
        Visible = true;
        SetPhysicsProcess(true);
        _collision?.SetDeferred("Disabled", false);
    }

    public void OnDespawn()
    {
        Visible = false;
        SetPhysicsProcess(false);
        _collision?.SetDeferred("Disabled", true);
        attachedPaddle = null;
    }

    public override void _Ready()
    {
        CollisionLayer = 1;
        CollisionMask = 1;

        _sprite = new Sprite2D();
        AddChild(_sprite);
        _sprite.Texture = CreateCircleTexture((int)radius * 2, Colors.White);

        _collision = new CollisionShape2D();
        AddChild(_collision);
        var shape = new CircleShape2D();
        shape.Radius = radius;
        _collision.Shape = shape;

    }

    public override void _PhysicsProcess(double delta)
    {
        if (attachedPaddle != null)
        {
            Position = attachedPaddle.Position - new Vector2(0, stuckGap + radius);
            if (Input.IsActionJustPressed("发射"))
            {
                attachedPaddle = null;
                Velocity = new Vector2(200, -speed);
                GD.Print($"Ball launched! Velocity: {Velocity}, Position: {Position}");
            }
        }
        else
        {
            var collision = MoveAndCollide(Velocity * (float)delta);
            if (collision != null)
            {
                Node2D collider = (Node2D)collision.GetCollider();
                if (collider is Brick brick)
                {
                    brick.Hit();
                    Velocity = Velocity.Bounce(collision.GetNormal());
                }
                else if (collider.Name == "TopWall")
                {
                    Velocity = new Vector2(Velocity.X, -Velocity.Y);
                }
                else if (collider.Name == "LeftWall" || collider.Name == "RightWall")
                {
                    Velocity = new Vector2(-Velocity.X, Velocity.Y);
                }
                else if (collider is Paddle paddle)
                {
                    Vector2 bounceDir = paddle.HitByBall(this);
                    Velocity = bounceDir * speed;
                }
            }   
        }
    }

    public void AttachTo(Paddle paddle)
    {
        attachedPaddle = paddle;
        isStuck = true;
    }

    private ImageTexture CreateCircleTexture(int size, Color color)
    {
        var image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
        int center = size / 2;
        int r = size / 2;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                if (dist <= r)
                    image.SetPixel(x, y, color);
            }
        }

        var texture = new ImageTexture();
        texture.SetImage(image);
        return texture;
    }
}