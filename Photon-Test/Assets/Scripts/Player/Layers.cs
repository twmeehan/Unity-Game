/*
 * Class Layers - used to get references to each layer. Each value is
 * 2^n where n is the layer number. This is beacuse layers use binary
 */
public enum Layers
{
    ground = 256,
    player = 512,
    room = 1024,
    collider = 2048,
    healing = 4096
}
