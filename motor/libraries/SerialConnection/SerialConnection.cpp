#include "SerialConnection.h"

SerialConnection::SerialConnection(HardwareSerial &serial, uint32_t speed, uint32_t timeout)
{
    m_Serial = &serial;
    m_Speed = speed;
    m_Timeout = timeout;
}

void SerialConnection::init() const
{
    m_Serial->begin(m_Speed);
    m_Serial->setTimeout(m_Timeout);
}

bool SerialConnection::connect() const
{
    while (!m_Serial)
    {
        delay(10);
    }

    return 1;
}

bool SerialConnection::dataAvailable() const
{
    return m_Serial->available();
}

bool SerialConnection::write(String payload) const
{
    m_Serial->write(payload.c_str());

    return 1;
}

String SerialConnection::readUntil(char terminator) const
{
    String value = m_Serial->readStringUntil(terminator);

    value.trim();

    return value;
}
