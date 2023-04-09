#include "Controller.h"

#define CMD_TURN_LEFT "tl"
#define CMD_TURN_RIGHT "tr"
#define CMD_FORWARD "fw"
#define CMD_BACKWARD "bw"

Controller::Controller(Engine &engine)
{
    m_Engine = &engine;

}

void Controller::init() const
{
    m_Engine->init();
}

bool Controller::execute(String *parameters, uint8_t parameterCount)
{
    if (parameterCount == 0)
    {
        return false;
    }

    if (parameters[0].equals(CMD_TURN_LEFT) && parameterCount >= 2)
    {
        m_Engine->steer(parameters[1].toInt());
    }
    else if (parameters[0].equals(CMD_TURN_RIGHT) && parameterCount >= 2)
    {
        m_Engine->steer(-parameters[1].toInt());
    }
    else if (parameters[0].equals(CMD_FORWARD) && parameterCount >= 2)
    {
        m_Engine->throttle(parameters[1].toInt());
    }
    else if (parameters[0].equals(CMD_BACKWARD) && parameterCount >= 2)
    {
        m_Engine->throttle(-parameters[1].toInt());
    }
    else
    {
        return false;
    }

    return true;
}
