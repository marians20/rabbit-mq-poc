#!/usr/bin/bash
sudo docker stop rabbitmq
sudo docker rm rabbitmq
sudo docker run \
  -d --name rabbitmq \
  --restart unless-stopped \
  -e "RABBITMQ_DEFAULT_USER=user" \
  -e "RABBITMQ_DEFAULT_PASSWORD=password" \
  -v /mnt/d/docker_data/rabbitmq/data:/var/lib/rabbitmq/mnesia/rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq-with-queue
# docker exec -it rabbitmq bash
# rabbitmqctl add_user {username} {password}
# rabbitmqctl set_user_tags {username} administrator
# rabbitmqctl set_permissions ...
