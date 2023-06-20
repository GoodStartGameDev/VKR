#!/bin/bash

tram_counter=1 # счетчик для номерации файлов с знаками трамвая
bus_counter=1 # счетчик для номерации файлов с знаками автобуса
crosswalk_counter=1 # счетчик для номерации файлов с зеброй
red_counter=1 # счетчик для номерации файлов с красным знаком
green_counter=1 # счетчик для номерации файлов с зеленым знаком
down_counter=1 # счетчик для номерации файлов со знаком "вниз"
up_counter=1 # счетчик для номерации файлов со знаком "вверх"
metro_counter=1 # счетчик для номерации файлов со знаком метро
for file in *; do # перебираем все файлы в текущей директории
  if [[ -f $file && "${file##*.}" == "txt" ]]; then # если файл является текстовым
    if $(grep -q "^2" "$file"); then # если файл содержит строку, начинающуюся с "2"
      mv "$file" "tram_sign${tram_counter}.txt" # переименовываем текстовый файл
      photo="${file%.*}" # получаем имя фото файла без расширения
      for ext in jpg png gif; do # перебираем возможные расширения фото
        if [[ -f "${photo}.${ext}" ]]; then # если файл фото существует
          mv "${photo}.${ext}" "tram_sign${tram_counter}.${ext}" # переименовываем фото
          break # выходим из цикла, если найдено соответствующее расширение
        fi
      done
      ((tram_counter++)) # увеличиваем счетчик для знаков трамвая
    elif $(grep -q "^3" "$file"); then # если файл содержит строку, начинающуюся с "3"
      mv "$file" "bus_sign${bus_counter}.txt" # переименовываем текстовый файл
      photo="${file%.*}" # получаем имя фото файла без расширения
      for ext in jpg png gif; do # перебираем возможные расширения фото
        if [[ -f "${photo}.${ext}" ]]; then # если файл фото существует
          mv "${photo}.${ext}" "bus_sign${bus_counter}.${ext}" # переименовываем фото
          break # выходим из цикла, если найдено соответствующее расширение
        fi
      done
      ((bus_counter++)) # увеличиваем счетчик для знаков автобуса
    elif $(grep -q "^4" "$file"); then # если файл содержит строку, начинающуюся с "4"
      mv "$file" "crosswalk${crosswalk_counter}.txt" # переименовываем текстовый файл
      photo="${file%.*}" # получаем имя фото файла без расширения
      for ext in jpg png gif; do # перебираем возможные расширения фото
        if [[ -f "${photo}.${ext}" ]]; then # если файл фото существует
          mv "${photo}.${ext}" "crosswalk${crosswalk_counter}.${ext}" # переименовываем фото
          break # выходим из цикла, если найдено соответствующее расширение
        fi
      done
      ((crosswalk_counter++)) # увеличиваем счетчик для зебры
    elif $(grep -q "^5" "$file"); then # если файл содержит строку, начинающуюся с "5"
      photo="${file%.*}" # получаем имя фото файла без расширения
      for ext in jpg png gif; do # перебираем возможные расширения фото
        if [[ -f "${photo}.${ext}" ]]; then # если файл фото существует
          case "$(head -n 1 "$file")" in
            "6") mv "$file" "red${red_counter}.txt"
                 mv "${photo}.${ext}" "red${red_counter}.${ext}"
                 ((red_counter++))
                 ;;
            "7") mv "$file" "green${green_counter}.txt"
                 mv "${photo}.${ext}" "green${green_counter}.${ext}"
                 ((green_counter++))
                 ;;
            "8") mv "$file" "down${down_counter}.txt"
                 mv "${photo}.${ext}" "down${down_counter}.${ext}"
                 ((down_counter++))
                 ;;
            "9") mv "$file" "up${up_counter}.txt"
                 mv "${photo}.${ext}" "up${up_counter}.${ext}"
                 ((up_counter++))
                 ;;
            *)   echo "Unknown code in file $file" >&2
                 ;;
          esac
          break # выходим из цикла, если найдено соответствующее расширение
        fi
      done
    elif $(grep -q "^1" "$file"); then # если файл содержит строку, начинающуюся с "1"
      mv "$file" "metro${metro_counter}.txt" # переименовываем текстовый файл
      photo="${file%.*}" # получаем имя фото файла без расширения
      for ext in jpg png gif; do # перебираем возможные расширения фото
        if [[ -f "${photo}.${ext}" ]]; then # если файл фото существует
          mv "${photo}.${ext}" "metro${metro_counter}.${ext}" # переименовываем фото
          break # выходим из цикла, если найдено соответствующее расширение
        fi
      done
      ((metro_counter++)) # увеличиваем счетчик для знаков метро
    fi
  fi
done