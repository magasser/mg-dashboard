#include "Arduino.h"
#include "SerialConnection.h"

SerialConnection::SerialConnection(Serial_ &serial, int speed, int timeout)
{
    m_Serial = &serial;
    m_Serial->begin(speed);
    m_Serial->setTimeout(timeout);
}

int SerialConnection::connect() const
{
    while (!m_Serial)
    {
    }

    return 1;
}

int SerialConnection::available() const
{
    return m_Serial->available();
}

int SerialConnection::write(const char *payload) const
{
    if (!m_Serial->availableForWrite())
    {
        return 0;
    }

    m_Serial->write(payload);

    return 1;
}

const char *SerialConnection::readUntil(char terminator) const
{
    return m_Serial->readStringUntil(terminator).c_str();
}
